using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComputerShare.Classes;
using ComputerShare.Interfaces;
using ComputerShare.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ComputerShare.Services.Zoopla
{
    public class ZooplaHousePriceService : IHousePriceService
    {
        private const string ZooplaHousePriceUrl =
            "http://api.zoopla.co.uk/api/v1/average_sold_prices.js?postcode={0}&output_type=outcode&area_type=postcodes&api_key={1}";

        private readonly IApiCaller _apiCaller;
        private string _apiKey;

        public ZooplaHousePriceService(
            IConfiguration appConfig,
            IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
            _apiKey = appConfig["zoopla-apiKey"];
        }

        public async Task<HousePriceInformation> GetHousePriceInfoForLocationAsync(string postcode, double latitude, double longitude)
        {
            var url = string.Format(ZooplaHousePriceUrl, postcode, _apiKey);
            var priceResult = await _apiCaller.GetHttpStringContentAsync<ZooplaAverageSoldPriceResult>(url);

            if (priceResult == null)
                throw new Exception($"Error with Zoopla Call: No results Returned for {postcode} - Is postcode correct?");

            var housePriceInfoList = GetHousePriceInformationList(priceResult);

            // Find the highest value postcode out of the items returned by Zoopla.
            // This is not necessarily the highest value postcode in reality - Zoopla do not give
            // an exhaustive list in the free trial...
            var highestHousePrice = housePriceInfoList.Aggregate((p1, p2) =>
                p1.AverageSoldPriceInLastYear > p2.AverageSoldPriceInLastYear ? p1 : p2);

            return highestHousePrice;
        }

        private List<HousePriceInformation> GetHousePriceInformationList(ZooplaAverageSoldPriceResult priceResult)
        {
            if (priceResult.areas == null ||
                priceResult.areas.Length == 0)
            {
                return new List<HousePriceInformation>();
            }

            var mapResults = priceResult.areas.Select(r =>
            {
                var relatedPostcode = "";

                if (!string.IsNullOrWhiteSpace(r.prices_url))
                {
                    // Tidy up the Postcode a bit..
                    var url = new Uri(r.prices_url);
                    var segments = url.Segments;
                    relatedPostcode = segments[segments.Length - 1];
                    relatedPostcode = relatedPostcode.Replace("-", " ").ToUpper();
                }

                return new HousePriceInformation(
                    r.number_of_sales_1year, 
                    r.average_sold_price_1year, 
                    relatedPostcode,
                    priceResult.latitude,
                    priceResult.longitude);
            }).ToList();

            return mapResults;
        }
    }
}