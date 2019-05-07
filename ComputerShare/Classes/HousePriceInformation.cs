
using ComputerShare.Interfaces;

namespace ComputerShare.Classes
{
    public class HousePriceInformation : IGeocodable
    {
        public double NumberOfSalesInLastYear { get; }
        public double AverageSoldPriceInLastYear { get; }
        public string Postcode { get; }
        public double Latitude { get; }
        public double Longitude { get; }
        public string LocationImageUrl { get; set; }
        public bool PostcodeHasBeenGeocoded { get; }

        public HousePriceInformation(
            double salesInLastYear,
            double averageSoldPriceInLastYear,
            string postcode,
            double latitude,
            double longitude)
        {
            NumberOfSalesInLastYear = salesInLastYear;
            AverageSoldPriceInLastYear = averageSoldPriceInLastYear;
            Postcode = postcode;
            Latitude = latitude;
            Longitude = longitude;

            PostcodeHasBeenGeocoded = true;
        }

        public void SetLocationImageUrl(string imageUrl)
        {
            LocationImageUrl = imageUrl;
        }
    }
}
