using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace ComputerShare.Classes
{
    public class CommandLineOptions
    {
        private List<string> _allPostcodes = new List<string>();

        [Option('p', "postcode", Required = false, HelpText = "The single postcode to be processed. The postcode must be wrapped in \" if a space char is used.")]
        public string Postcode { get; set; }

        [Option('f', "postcodeFile", Required = false, HelpText = "The postcode file to be processed. (Postcodes on Consecutive Lines)")]
        public string InputFile { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        public void SetPostcodeList(List<string> postcodes)
        {
            _allPostcodes = postcodes;
        }

        // This combines the single and multiple postcodes that might be provided
        // for convenience.
        public List<string> GetPostcodeList()
        {
            var completeList = new List<string>();

            if (_allPostcodes != null)
                completeList.AddRange(_allPostcodes);

            if (!string.IsNullOrWhiteSpace(Postcode))
            {
                completeList.Add(Postcode);
            }

            return completeList;
        }
    }
}
