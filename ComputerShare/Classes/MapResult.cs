
using ComputerShare.Interfaces;

namespace ComputerShare.Classes
{
    public class MapResult : IGeocodable
    {
        public string Postcode { get; private set; }
        public bool PostcodeHasBeenGeocoded { get; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string Town { get; private set; }

        public string LocationImageUrl { get; private set; }

        public HousePriceInformation HousePrice { get; set; }

        public MapResult(
            string postcode,
            bool isValid)
        {
            Postcode = postcode;
            PostcodeHasBeenGeocoded = isValid;
        }

        public MapResult(
            string postcode,
            double latitude,
            double longitude,
            string town)
        {
            Postcode = postcode;
            Latitude = latitude;
            Longitude = longitude;
            Town = town;
            PostcodeHasBeenGeocoded = true;
        }

        public void SetLocationImageUrl(string imageUrl)
        {
            LocationImageUrl = imageUrl;
        }

        public void SetHousePriceInformation(HousePriceInformation priceInfos)
        {
            HousePrice = priceInfos;
        }
    }
}