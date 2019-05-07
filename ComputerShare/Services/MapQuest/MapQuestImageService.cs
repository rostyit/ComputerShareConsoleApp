using System;
using System.Threading.Tasks;
using ComputerShare.Interfaces;
using ComputerShare.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ComputerShare.Services.MapQuest
{
    /// <summary>
    /// Using the MapQuest API for Static Map Generation (https://developer.mapquest.com/documentation)
    /// MapQuest allows up to 15,000 free transactions per month
    /// </summary>
    public class MapQuestImageService : IMapImageService
    {
        private const string MapImageLookupUrl =
            "https://www.mapquestapi.com/staticmap/v5/map?key={0}&center={1},{2}&zoom=15";

        private readonly IApiCaller _apiCaller;
        private string _apiKey;

        public bool LatAndLongRequiredByService { get { return true; } }

        public MapQuestImageService(
            IConfiguration appConfig,
            IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
            _apiKey = appConfig["mapquest-apiKey"];
        }

        public async Task<string> GetMapImageForLocationAsync(
            string postcode,
            double latitude, 
            double longitude)
        {
            try
            {
                var url = string.Format(MapImageLookupUrl, _apiKey, latitude, longitude);
                var bytes = await _apiCaller.GetHttpByteArrayAsync(url);

                if (bytes == null)
                    throw new Exception($"Error with MapQuest Call: No results Returned for {postcode} - Are Lat and Long Correct?");

                return url;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
