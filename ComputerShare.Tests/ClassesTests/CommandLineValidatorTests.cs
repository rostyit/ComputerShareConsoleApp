using System.IO;
using System.Linq;
using System.Reflection;
using ComputerShare.Classes;
using NUnit.Framework;

namespace ComputerShare.Tests.ClassesTests
{
    public class CommandLineValidatorTests
    {
        [TestCase("M1 1AA")]
        [TestCase("M60 1NW")]
        [TestCase("CR2 6XH")]
        [TestCase("DN55 1PT")]
        [TestCase("W1A 1HQ")]
        [TestCase("EC1A 1BB")]
        [TestCase("EC1A1BB")]
        public void AValidPostcode_DoesNotCause_AValidationError(string postcode)
        {
            var options = GetCommandLineOptions(postcode);
            var commandLineValidator = new CommandLineValidator();
            Assert.That(string.IsNullOrWhiteSpace(commandLineValidator.Validate(options)));
        }

        [TestCase("Dave")]
        [TestCase("123456")]
        [TestCase("-")]
        [TestCase("EC1A1BB1")]
        public void ANonValidPostcode_Causes_AValidationError(string postcode)
        {
            var options = GetCommandLineOptions(postcode);
            var commandLineValidator = new CommandLineValidator();
            var errorMessage = commandLineValidator.Validate(options);
            Assert.That(!string.IsNullOrWhiteSpace(errorMessage), errorMessage);
        }

        [Test]
        public void AValidFilePath_WithValidPostcodes_DoesNotCause_AValidationError()
        {
            var validFilePath = GetNamedTestFilePath("TestPostcodes.txt");
            
            var options = GetCommandLineOptions("", validFilePath);

            var commandLineValidator = new CommandLineValidator();
            var errorMessage = commandLineValidator.Validate(options);
            Assert.That(string.IsNullOrWhiteSpace(errorMessage), errorMessage);
            Assert.That(commandLineValidator.FilePostcodes.Count == 3);
        }

        [Test]
        public void AValidFilePath_WithNonValidPostcodes_Causes_AValidationError()
        {
            var validFilePath = GetNamedTestFilePath("NonValidPostcodes.txt");

            var options = GetCommandLineOptions("", validFilePath);

            var commandLineValidator = new CommandLineValidator();
            var errorMessage = commandLineValidator.Validate(options);
            Assert.That(!string.IsNullOrWhiteSpace(errorMessage), errorMessage);
        }

        [Test]
        public void AValidFilePath_WithNoPostcodes_Causes_AValidationError()
        {
            var validFilePath = GetNamedTestFilePath("Empty.txt");

            var options = GetCommandLineOptions("", validFilePath);

            var commandLineValidator = new CommandLineValidator();
            var errorMessage = commandLineValidator.Validate(options);
            Assert.That(!string.IsNullOrWhiteSpace(errorMessage), errorMessage);
        }

        [Test]
        public void ANonValidFilePath_Causes_AValidationError()
        {
            var options = GetCommandLineOptions("", "c:\\NOT_HERE.txt");

            var commandLineValidator = new CommandLineValidator();
            var errorMessage = commandLineValidator.Validate(options);
            Assert.That(!string.IsNullOrWhiteSpace(errorMessage), errorMessage);
        }

        private string GetNamedTestFilePath(string requiredTestFile)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var assemblyFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            assemblyFile = assemblyFile.Substring(0, assemblyFile.IndexOf("\\ComputerShare.Tests"));

            var resourceNames = executingAssembly.GetManifestResourceNames();
            var foundFilePath = resourceNames.FirstOrDefault(r => r.Contains(requiredTestFile));
            var filePath = foundFilePath.Replace(".", "\\").Replace("\\txt", ".txt").Replace("ComputerShare\\Tests", "ComputerShare.Tests");

            return $"{assemblyFile}\\{filePath}";
        }
        
        private CommandLineOptions GetCommandLineOptions(string postcode, string postcodeFilePath = "")
        {
            return new CommandLineOptions
            {
                Postcode = postcode,
                InputFile = postcodeFilePath
            };
        }
    }
}