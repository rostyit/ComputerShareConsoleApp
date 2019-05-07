using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ComputerShare.Classes;
using ComputerShare.Interfaces;
using ComputerShare.Orchestrators;
using ComputerShare.Orchestrators.Interfaces;
using ComputerShare.Services;
using ComputerShare.Services.Interfaces;
using ComputerShare.Services.MapQuest;
using ComputerShare.Services.PostcodeIo;
using ComputerShare.Services.Zoopla;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ComputerShare
{
    public class Program
    {
        private static IServiceProvider _serviceProvider;

        static async Task Main(string[] args)
        {
            try
            {
                var appConfig = BuildConfiguration();
                RegisterServices(appConfig);

                var commandLineOptions = new CommandLineOptions();

                if (!CommandLine.Parser.Default.ParseArguments(args, commandLineOptions))
                {
                    OutputConsoleInformation("There was a problem with the argument supplied");
                    return;
                }

                var commandLineValidator = _serviceProvider.GetService<ICommandLineValidator>();
                var validationErrors = commandLineValidator.Validate(commandLineOptions);

                if (!string.IsNullOrWhiteSpace(validationErrors))
                {
                    OutputConsoleInformation($"There have been argument validation errors: {validationErrors}");
                    return;
                }

                commandLineOptions.SetPostcodeList(commandLineValidator.FilePostcodes);

                // We can start using the postcodes to look up house prices for the surrounding area...
                var postcodeOrchestrator = _serviceProvider.GetService<ILookupOrchestrator>();
                var fileGenerationInfo = await postcodeOrchestrator.LookupPostcodeDetailsAsync(commandLineOptions.GetPostcodeList());

                OutputConsoleInformation(fileGenerationInfo);
            }
            catch (ApiException e)
            {
                OutputConsoleInformation($"Error using an API: {e.Message} {Environment.NewLine} [StatusCode: {e.StatusCode}] [Content: {e.StringContent}]");
            }
            catch (Exception e)
            {
                OutputConsoleInformation($"Error: {e.Message}");
            }
            finally
            {
                DisposeServices();
            }
        }


        private static void OutputConsoleInformation(string info)
        {
            Console.WriteLine(info);
            Console.ReadLine();
        }

        // Get hold of the appsettings available to us.
        private static IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            return configuration;
        }

        private static void RegisterServices(IConfiguration appConfig)
        {
            var collection = new ServiceCollection();

            collection.AddSingleton<IConfiguration>(sp => appConfig);
            collection.AddSingleton<HttpClient>(sp => new HttpClient());
            collection.AddScoped<IApiCaller, ApiCaller>();
            collection.AddScoped<ICommandLineValidator, CommandLineValidator>();

            collection.AddScoped<ILookupOrchestrator, LookupOrchestrator>();

            collection.AddScoped<IGeocodingService, PostcodeIoGeocodingService>();
            collection.AddScoped<IMapImageService, MapQuestImageService>();
            collection.AddScoped<IHousePriceService, ZooplaHousePriceService>();
            collection.AddScoped<IHtmlGeneratorService, HtmlGeneratorService>();

            _serviceProvider = collection.BuildServiceProvider();
        }

        private static void DisposeServices()
        {
            switch (_serviceProvider)
            {
                case null:
                    return;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }
    }
}
