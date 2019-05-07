using ComputerShare.Orchestrators.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComputerShare.Classes;
using ComputerShare.Interfaces;
using ComputerShare.Services.Interfaces;

namespace ComputerShare.Orchestrators
{
    public class LookupOrchestrator : ILookupOrchestrator
    {
        private readonly IGeocodingService _geocodingService;
        private readonly IMapImageService _mapImageService;
        private readonly IHousePriceService _housePriceService;
        private readonly IHtmlGeneratorService _htmlGeneratorService;

        public LookupOrchestrator(
            IGeocodingService geocodingService,
            IMapImageService mapImageService,
            IHousePriceService housePriceService,
            IHtmlGeneratorService htmlGeneratorService)
        {
            _geocodingService = geocodingService;
            _mapImageService = mapImageService;
            _housePriceService = housePriceService;
            _htmlGeneratorService = htmlGeneratorService;
        }

        public async Task<string> LookupPostcodeDetailsAsync(List<string> postcodes)
        {
            if (postcodes == null || !postcodes.Any())
                return "File Could not be Generated - No postcodes provided.";

            var mapResults = await _geocodingService.BulkGeocodePostcodesAsync(postcodes);

            if (mapResults == null || !mapResults.Any())
                return "File Could not be Generated - Postcodes could not be geocoded.";

            // Get the original lookup search image.
            await ObtainMappingImages(mapResults.ToList<IGeocodable>());

            var sortedMapResults = await ObtainHousePrices(mapResults);

            // Now obtain the images for the most expensive house location
            await ObtainMappingImages(sortedMapResults.Select(r => r.HousePrice).ToList<IGeocodable>());

            var generatedFilePath = GenerateHtmlFromMapResults(sortedMapResults);

            return $"Postcode Information Generated! - Check file {generatedFilePath}";
        }

        private async Task ObtainMappingImages(List<IGeocodable> results)
        {
            for (var index = 0; index < results.Count; index++)
            {
                var geocodable = results[index];

                if (geocodable == null)
                    continue;

                // we can only get images that have a valid lat and long...
                if (!_mapImageService.LatAndLongRequiredByService ||
                    _mapImageService.LatAndLongRequiredByService && geocodable.PostcodeHasBeenGeocoded)
                {
                    var imageUrl = await _mapImageService.GetMapImageForLocationAsync(
                        geocodable.Postcode,
                        geocodable.Latitude,
                        geocodable.Longitude);

                    geocodable.SetLocationImageUrl(imageUrl);
                }
            }
        }

        private async Task<List<MapResult>> ObtainHousePrices(List<MapResult> mapResults)
        {
            for (var index = 0; index < mapResults.Count; index++)
            {
                if (mapResults[index].PostcodeHasBeenGeocoded)
                {
                    var housePriceResult = await _housePriceService.GetHousePriceInfoForLocationAsync(
                        mapResults[index].Postcode,
                        mapResults[index].Latitude,
                        mapResults[index].Longitude);

                    mapResults[index].SetHousePriceInformation(housePriceResult);
                }
            }

            if (mapResults.Count <= 1)
                return mapResults;

            return mapResults.OrderByDescending(m => m.HousePrice.AverageSoldPriceInLastYear).ToList();
        }

        private string GenerateHtmlFromMapResults(List<MapResult> mapResults)
        {
            return _htmlGeneratorService.Generate(mapResults);
        }
    }
}
