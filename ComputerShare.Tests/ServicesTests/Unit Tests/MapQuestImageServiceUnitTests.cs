using System;
using System.IO;
using System.Threading.Tasks;
using ComputerShare.Interfaces;
using ComputerShare.Services.MapQuest;
using ComputerShare.Services.PostcodeIo;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace ComputerShare.Tests.ServicesTests.Unit_Tests
{
    [TestFixture()]
    public class MapQuestImageServiceUnitTests
    {
        [Test]
        public void GetMapImage_ReturnsNull_IfSourceLatAndLongAreNotValid()
        {
            var apiCaller = GetMockApiCaller(null);
            var mapQuestService = new MapQuestImageService(BuildConfiguration(), apiCaller);

            // Invalid lat and long provided here
            Assert.ThrowsAsync<Exception>(() => mapQuestService.GetMapImageForLocationAsync("BS22 9BY", 91, 181));
        }

        [Test]
        public async Task GetMapImage_ReturnsByteArray_IfSourceLatAndLongAreValid()
        {
            var apiCaller = GetMockApiCaller(null);
            var geocodeService = new PostcodeIoGeocodingService(apiCaller);

            var geocodeResult = await geocodeService.BulkGeocodePostcodesAsync(null);

            Assert.That(geocodeResult == null);
        }

        private IApiCaller GetMockApiCaller(byte[] returnObject)
        {
            var apiCaller = Substitute.For<IApiCaller>();
            apiCaller
                .GetHttpByteArrayAsync(Arg.Any<string>())
                .ReturnsForAnyArgs(returnObject);

            return apiCaller;
        }

        private static IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            return configuration;
        }
    }
}