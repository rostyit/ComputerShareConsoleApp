using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComputerShare.Classes;
using ComputerShare.Interfaces;
using ComputerShare.Services.Interfaces;

namespace ComputerShare.Services.PostcodeIo
{
    /// <summary>
    /// Using the Postcode.io api for geocoding (https://postcodes.io/docs)
    /// </summary>
    public class PostcodeIoGeocodingService : IGeocodingService
    {
        private const string BulkPostcodeLookupUrl = "https://api.postcodes.io/postcodes";

        private readonly IApiCaller _apiCaller;

        public PostcodeIoGeocodingService(
            IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
        }

        public async Task<List<MapResult>> BulkGeocodePostcodesAsync(List<string> postcodes)
        {
            if (postcodes == null || !postcodes.Any())
                return null;

            var mapResults = await GeocodePostcodes(postcodes);

            return mapResults;
        }

        private async Task<List<MapResult>> GeocodePostcodes(List<string> postcodes)
        {
            List<MapResult> mapResults;
            try
            {
                var postcodeioResponse =
                    await _apiCaller.PostJsonObject<Postcodeio_Result<List<Postcodeio_Query<Postcodeio_Postcode>>>>(
                        BulkPostcodeLookupUrl,
                        new Postcodeio_BulkPostcodeLookup() {postcodes = postcodes.ToArray()});

                if (postcodeioResponse.status != 200)
                {
                    throw new Exception($"Error with PostcodeIo Call: {postcodeioResponse.status}");
                }

                if (postcodeioResponse.result == null ||
                    postcodeioResponse.result.Count == 0)
                {
                    throw new Exception($"Error with PostcodeIo Call: No results Returned - Are Postcodes Correct?");
                }

                mapResults = GetMapResultList(postcodeioResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine($"PostcodeIoGeocodingService: {e}");
                throw;
            }

            return mapResults;
        }

        private List<MapResult> GetMapResultList(Postcodeio_Result<List<Postcodeio_Query<Postcodeio_Postcode>>> postcodeioResponse)
        {
            var mapResults = postcodeioResponse.result.Select(r =>
            {
                var postcodeResult = r.result;
                if (postcodeResult == null)
                {
                    // This postcode could not be geocoded - NOT VALID
                    return new MapResult(r.query, false);
                }

                return new MapResult(
                    r.query,
                    postcodeResult.latitude,
                    postcodeResult.longitude,
                    postcodeResult.parish);
            }).ToList();

            return mapResults;
        }
    }
}
