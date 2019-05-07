using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ComputerShare.Classes;
using ComputerShare.Interfaces;
using ComputerShare.Services.PostcodeIo;
using ComputerShare.Services.Zoopla;
using NUnit.Framework;

namespace ComputerShare.Tests.ClassesTests
{
    // Integration Tests to ensure our source APIs can return what we need...
    //TODO: There needs to be some sad path tests available in here.
    [TestFixture]
    public class HttpCallerIntegrationTests
    {
        private IApiCaller _apiCaller;

        [OneTimeSetUp]
        public void SetupFixture()
        {
            var httpClient = new HttpClient();
            _apiCaller = new ApiCaller(httpClient);
        }

        [Test]
        public async Task ApiCaller_ReturnsGeoEncodedInfo_FromPostcodesio_ForASinglePostcode()
        {
            // This gets the required geo encoded info for a known postcode
            var postcodeioResponse = await _apiCaller.GetHttpStringContentAsync<Postcodeio_Result<Postcodeio_Postcode>>(
                @"https://api.postcodes.io/postcodes/BS22 9BY");

            Assert.That(postcodeioResponse.status == 200);
            Assert.That(postcodeioResponse.result != null);
            Assert.That(postcodeioResponse.result.parish == "Wick St. Lawrence");
        }

        [Test]
        public async Task ApiCaller_ReturnsGeoEncodedInfo_FromPostcodesio_ForBulkPostcodes()
        {
            // This gets the required geo encoded info for a bulk list of postcodes
            var postcodeLookups = new[] {"BS22 9BY", "BS21 6PE"};
            var postcodeioResponse = await _apiCaller.PostJsonObject<Postcodeio_Result<List<Postcodeio_Query<Postcodeio_Postcode>>>>(
                @"https://api.postcodes.io/postcodes",
                new Postcodeio_BulkPostcodeLookup() {postcodes = postcodeLookups});

            Assert.That(postcodeioResponse.status == 200);
            Assert.That(postcodeioResponse.result.Count == 2);
        }

        [Test]
        public async Task ApiCaller_ReturnsAStaticMap_FromMapQuest()
        {
            TidyUpTestFiles();

            Assert.That(!File.Exists("TestImage.jpg"));

            // This tries to get a static image of a know location and save it to file
            var bytes = await _apiCaller.GetHttpByteArrayAsync(
                @"https://www.mapquestapi.com/staticmap/v5/map?key=0CGE6xF09R6IFdsxjPeUwG2CvKUHQgnk&center=51.3603,-2.9293&zoom=15");

            await File.WriteAllBytesAsync("TestImage.jpg", bytes);

            Assert.That(File.Exists("TestImage.jpg"));

            TidyUpTestFiles();
        }

        [Test]
        public async Task ApiCaller_ReturnsHousePriceInformation_FromZoopla_ForSinglePostcode()
        {
            var zooplaResponse = await _apiCaller.GetHttpStringContentAsync<ZooplaAverageSoldPriceResult>(
                @"http://api.zoopla.co.uk/api/v1/average_sold_prices.js?postcode=BS22+9BY&output_type=outcode&area_type=postcodes&api_key=e5qyexfa2umkkn4e55vd6x9m");

            Assert.That(zooplaResponse.postcode == "BS22");
        }

        private void TidyUpTestFiles()
        {
            if (File.Exists("TestImage.jpg"))
            {
                File.Delete("TestImage.jpg");
            }
        }
    }
}
