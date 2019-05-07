using System.Collections.Generic;
using System.IO;
using ComputerShare.Classes;
using ComputerShare.Services;
using NUnit.Framework;

namespace ComputerShare.Tests.ServicesTests.Unit_Tests
{
    [TestFixture()]
    public class HtmlGeneratorServiceTests
    {
        [Test]
        public void Html_BuildsArticles_BasedOnMapResults_AndArticleTemplate()
        {
            var filePath = "";
            try
            {
                var htmlGen = new HtmlGeneratorService();
                filePath = htmlGen.Generate(GetMapResults());

                Assert.That(!string.IsNullOrWhiteSpace(filePath));
                Assert.That(File.Exists(filePath));
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(filePath))
                    File.Delete(filePath);
            }
        }

        private List<MapResult> GetMapResults()
        {
            var resultList = new List<MapResult>();

            for (int index = 0; index < 8; index++)
            {
                resultList.Add(new MapResult("BS22 9BY", 22, -2, "Wick st Lawrence"));
            }

            return resultList;
        }
    }
}
