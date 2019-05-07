using System.Collections.Generic;
using ComputerShare.Classes;

namespace ComputerShare.Interfaces
{
    public interface ICommandLineValidator
    {
        List<string> FilePostcodes { get; }
        string Validate(CommandLineOptions commandLineOptions);
    }
}
