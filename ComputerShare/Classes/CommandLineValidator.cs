using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ComputerShare.Interfaces;

namespace ComputerShare.Classes
{
    public class CommandLineValidator : ICommandLineValidator
    {
        private const string PostcodeRegex =
            @"([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})";

        public List<string> FilePostcodes { get; private set; }

        public string Validate(CommandLineOptions commandLineOptions)
        {
            var strBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(commandLineOptions.Postcode))
                strBuilder.AppendLine(ValidatePostcode(commandLineOptions.Postcode));

            if (!string.IsNullOrWhiteSpace(commandLineOptions.InputFile))
                strBuilder.AppendLine(ValidatePostcodeFile(commandLineOptions.InputFile));
            
            return strBuilder.ToString();
        }

        private string ValidatePostcode(string postcode)
        {
            if (string.IsNullOrWhiteSpace(postcode))
                return $"The Postcode is empty and can not be validated.";

            if ((postcode.Contains(" ") && postcode.Length > 8) ||
                (!postcode.Contains(" ") && postcode.Length > 7))
                return $"The Postcode: {postcode} is too Long.";

            var match = Regex.Match(
                postcode, 
                PostcodeRegex, 
                RegexOptions.IgnoreCase);

            // Here we check the Match instance.
            if (!match.Success)
            {
                return $"The Postcode: {postcode} has not been successfully validated.";
            }

            return "";
        }

        private string ValidatePostcodes(string postcodeFilePath, List<string> postcodes)
        {
            if (!postcodes.Any())
                return $"The Postcode File presented: [{postcodeFilePath}] Does not contain any data!";

            var strBuilder = new StringBuilder();
            postcodes.ForEach(pc =>
            {
                var error = ValidatePostcode(pc);
                if (!string.IsNullOrWhiteSpace(error))
                    strBuilder.AppendLine(error);
            });

            if (strBuilder.Length > 0)
            {
                // There are errors which should be displayed against the fileName
                strBuilder.Insert(0, $"The file: [{postcodeFilePath}] returned the following postcode validation errors.");
            }
            else
            {
                // All postcodes presented in this file are good, make them available for external use
                FilePostcodes = postcodes;
            }

            return strBuilder.ToString();
        }

        // Check that the file exists, that it can be opened and that we can open it
        private string ValidatePostcodeFile(string postcodeFilePath)
        {
            if (!File.Exists(postcodeFilePath))
                return $"The Postcode File presented: {postcodeFilePath} Does not exist!";

            var postcodeList = new List<string>();
            const int bufferSize = 128; // This defaults to 1024 otherwise

            try
            {
                using (var fileStream = File.OpenRead(postcodeFilePath))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, bufferSize))
                {
                    var line = "";
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        postcodeList.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                return $"There was an exception opening and reading the file: {postcodeFilePath} [{e.Message}]";
            }

            return ValidatePostcodes(postcodeFilePath, postcodeList);
        }

    }
}
