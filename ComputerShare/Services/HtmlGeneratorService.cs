using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ComputerShare.Classes;
using ComputerShare.Services.Interfaces;

namespace ComputerShare.Services
{
    public class HtmlGeneratorService : IHtmlGeneratorService
    {
        public string Generate(List<MapResult> mapResults)
        {
            var htmlString = "";
            var articleTemplate = ReadHtmlTemplateFromFile("ArticleFormatTemplate.txt");

            var articleSb = new StringBuilder();

            foreach (var mapResult in mapResults)
            {
                articleSb.Append(BuildArticleFromTemplate(mapResult, articleTemplate));
            }

            if (articleSb.Length == 0)
            {
                return htmlString;
            }

            var housePriceTemplate = ReadHtmlTemplateFromFile("HousePriceTemplate.txt");
            var documentSb = new StringBuilder(housePriceTemplate);
            documentSb.Replace("##Articles##", articleSb.ToString());
        
            return SaveHtmlToFile(documentSb.ToString());
        }

        /// <summary>
        /// The article Template will have the following items that need replacement with MapResult
        /// Values: ##ImageUrl##, ##Postcode##, ##ExpensivePostcode##, ##ExpensiveImageUrl##, ##AveHousePrice##, ##NumberOfSales##
        /// </summary>
        /// <param name="mapResult"></param>
        /// <param name="articleTemplate"></param>
        /// <returns></returns>
        private string BuildArticleFromTemplate(MapResult mapResult, string articleTemplate)
        {
            var sb = new StringBuilder(articleTemplate);
            sb.Replace("##ImageUrl##", mapResult.LocationImageUrl);
            sb.Replace("##Postcode##", mapResult.Postcode);
            sb.Replace("##ExpensivePostcode##", mapResult.HousePrice == null ? "N/A" : mapResult.HousePrice.Postcode);
            sb.Replace("##ExpensiveImageUrl##", mapResult.HousePrice == null ? "N/A" : mapResult.HousePrice.LocationImageUrl);
            sb.Replace("##AveHousePrice##", mapResult.HousePrice == null ? "N/A" : mapResult.HousePrice.AverageSoldPriceInLastYear.ToString());
            sb.Replace("##NumberOfSales##", mapResult.HousePrice == null ? "N/A" : mapResult.HousePrice.NumberOfSalesInLastYear.ToString());

            return sb.ToString();
        }

        private string ReadHtmlTemplateFromFile(string fileName)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var assemblyFile = Path.GetDirectoryName(executingAssembly.Location);
            assemblyFile = assemblyFile.Substring(0, assemblyFile.LastIndexOf("\\ComputerShare"));

            var resourceNames = executingAssembly.GetManifestResourceNames();
            var foundFilePath = resourceNames.FirstOrDefault(r => r.Contains(fileName));
            var filePath = foundFilePath.Replace(".", "\\").Replace("\\txt", ".txt");

            var fullFilePath = $"{assemblyFile}\\{filePath}";
            var htmlTemplate = File.ReadAllText(fullFilePath);
            return htmlTemplate;
        }

        private string SaveHtmlToFile(string html)
        {
            var fileName = $"HousepriceOutput_{DateTime.Now}.html";
            fileName = fileName.Replace("/", "").Replace(":", "");
            File.WriteAllText(fileName, html);

            var fileInfo = new FileInfo(fileName);
            return fileInfo.FullName;
        }
    }
}
