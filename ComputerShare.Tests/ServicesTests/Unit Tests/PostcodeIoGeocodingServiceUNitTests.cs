using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComputerShare.Interfaces;
using ComputerShare.Services.PostcodeIo;
using NSubstitute;
using NUnit.Framework;

namespace ComputerShare.Tests.ServicesTests.Unit_Tests
{
    [TestFixture()]
    public class PostcodeIoGeocodingServiceUnitTests
    {
        [Test]
        public async Task BulkLookupPostcodes_ReturnsNull_IfSourcePostcodeListIsNull()
        {
            var apiCaller = GetMockApiCaller(null);
            var geocodeService = new PostcodeIoGeocodingService(apiCaller);

            var geocodeResult = await geocodeService.BulkGeocodePostcodesAsync(null);

            Assert.That(geocodeResult == null);
        }

        [Test]
        public async Task BulkLookupPostcodes_ReturnsNull_IfSourcePostcodeListIsEmpty()
        {
            var apiCaller = GetMockApiCaller(null);
            var geocodeService = new PostcodeIoGeocodingService(apiCaller);

            var geocodeResult = await geocodeService.BulkGeocodePostcodesAsync(new List<string>());

            Assert.That(geocodeResult == null);
        }

        [Test]
        public void BulkLookupPostcodes_ThrowsException_IfErrorResponseStatus()
        {
            var apiCallerRetObj = new Postcodeio_Result<List<Postcodeio_Query<Postcodeio_Postcode>>>();
            apiCallerRetObj.status = 400;
            var apiCaller = GetMockApiCaller(apiCallerRetObj);
            var geocodeService = new PostcodeIoGeocodingService(apiCaller);

            Assert.ThrowsAsync<Exception>(async () => await geocodeService.BulkGeocodePostcodesAsync(new List<string>() { "BS21 6PE" }));
        }

        [Test]
        public void BulkLookupPostcodes_ThrowsException_IfReturnedResultsAreNull()
        {
            var apiCallerRetObj = new Postcodeio_Result<List<Postcodeio_Query<Postcodeio_Postcode>>>();
            apiCallerRetObj.status = 200;
            var apiCaller = GetMockApiCaller(apiCallerRetObj);
            var geocodeService = new PostcodeIoGeocodingService(apiCaller);

            Assert.ThrowsAsync<Exception>(async () => await geocodeService.BulkGeocodePostcodesAsync(new List<string>() { "BS21 6PE" }));
        }

        [Test]
        public void BulkLookupPostcodes_ThrowsException_IfReturnedResultsAreEmpty()
        {
            var apiCallerRetObj = new Postcodeio_Result<List<Postcodeio_Query<Postcodeio_Postcode>>>();
            apiCallerRetObj.status = 200;
            apiCallerRetObj.result = new List<Postcodeio_Query<Postcodeio_Postcode>>();
            var apiCaller = GetMockApiCaller(apiCallerRetObj);
            var geocodeService = new PostcodeIoGeocodingService(apiCaller);

            Assert.ThrowsAsync<Exception>(async () => await geocodeService.BulkGeocodePostcodesAsync(new List<string>() { "BS21 6PE" }));
        }

        [Test]
        public async Task BulkLookupPostcodes_ReturnsAListOfMapResults_IfSuccessful()
        {
            var apiCallerRetObj = new Postcodeio_Result<List<Postcodeio_Query<Postcodeio_Postcode>>>();
            apiCallerRetObj.status = 200;
            apiCallerRetObj.result = new List<Postcodeio_Query<Postcodeio_Postcode>>();
            apiCallerRetObj.result.Add(new Postcodeio_Query<Postcodeio_Postcode>() { query = "BS22 9BY", result = new Postcodeio_Postcode() });
            apiCallerRetObj.result.Add(new Postcodeio_Query<Postcodeio_Postcode>() { query = "BS21 6PE", result = new Postcodeio_Postcode() });
            var apiCaller = GetMockApiCaller(apiCallerRetObj);
            var geocodeService = new PostcodeIoGeocodingService(apiCaller);

            var mapResults = await geocodeService.BulkGeocodePostcodesAsync(new List<string>() { "BS21 6PE", "BS22 9BY" });

            Assert.That(mapResults != null);
            Assert.That(mapResults.Count == 2);
        }

        [Test]
        public async Task BulkLookupPostcodes_ReturnsAListOfMapResults_WithNonGeocodedResults_MarkedAsSuch()
        {
            var apiCallerRetObj = new Postcodeio_Result<List<Postcodeio_Query<Postcodeio_Postcode>>>();
            apiCallerRetObj.status = 200;
            apiCallerRetObj.result = new List<Postcodeio_Query<Postcodeio_Postcode>>();
            apiCallerRetObj.result.Add(new Postcodeio_Query<Postcodeio_Postcode>() { query = "BS22 9BY", result = new Postcodeio_Postcode() });
            apiCallerRetObj.result.Add(new Postcodeio_Query<Postcodeio_Postcode>() { query = "AA123 AAA", result = null });
            var apiCaller = GetMockApiCaller(apiCallerRetObj);
            var geocodeService = new PostcodeIoGeocodingService(apiCaller);

            var mapResults = await geocodeService.BulkGeocodePostcodesAsync(new List<string>() { "AA123 AAA", "BS22 9BY" });

            Assert.That(mapResults != null);
            Assert.That(mapResults.Count == 2);
            Assert.That(mapResults.FirstOrDefault(mr => mr.PostcodeHasBeenGeocoded == false) != null);  //There should be 1 that could NOT be geocoded
            Assert.That(mapResults.FirstOrDefault(mr => mr.PostcodeHasBeenGeocoded == true) != null);  //There should be 1 that could be geocoded
        }

        private IApiCaller GetMockApiCaller(Postcodeio_Result<List<Postcodeio_Query<Postcodeio_Postcode>>> returnObject)
        {
            var apiCaller = Substitute.For<IApiCaller>();
            apiCaller
                .PostJsonObject<Postcodeio_Result<List<Postcodeio_Query<Postcodeio_Postcode>>>>(Arg.Any<string>(), Arg.Any<object>())
                .ReturnsForAnyArgs(returnObject);

            return apiCaller;
        }
    }
}
