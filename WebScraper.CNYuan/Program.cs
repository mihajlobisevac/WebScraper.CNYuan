using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WebScraper.CNYuan.Common;

namespace WebScraper.CNYuan
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            ServiceProvider serviceProvider = RegisterServices(args);
            IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
            FileOutput fileOutput = new(configuration);

            var response = WebRequests.GetInitialResponse();
            var htmlDocument = response.GetHtmlDocument();
            var currencies = htmlDocument.GetCurrencies();

            Extensions.Output($"{currencies.Count} currencies loaded...");

            foreach (var currency in currencies)
            {
                //if it takes too long to scrap, set "scrapAllPages" parameter to false
                var records = htmlDocument.ScrapCurrencyData(currency, true);

                if(records.Count > 0) fileOutput.CreateRecordFile(records);
            }

            serviceProvider.Dispose();

            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();

                    IHostEnvironment env = hostingContext.HostingEnvironment;

                    configuration
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

                    IConfigurationRoot configurationRoot = configuration.Build();
                });

        private static IConfiguration SetupConfiguration(string[] args)
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
        }

        private static ServiceProvider RegisterServices(string[] args)
        {
            IConfiguration configuration = SetupConfiguration(args);
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(configuration);

            return serviceCollection.BuildServiceProvider();
        }
    }
}
