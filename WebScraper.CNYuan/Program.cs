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

            var response = GetInitialResponse();
            var htmlDocument = response.GetHtmlDocument();
            var currencies = htmlDocument.GetCurrencies();

            Extensions.Output($"{currencies.Count} currencies loaded...");

            foreach (var currency in currencies)
            {
                HttpWebRequest request = CreateWebRequest(UrlConstants.BankOfChina);
                htmlDocument = request.GetHtmlDocumentWithData(currency);

                Extensions.Output($"{currency} currency record scraping in progress...");

                var listOfRows = htmlDocument.GetTableRows();
                var numberOfPages = htmlDocument.GetNumberOfPages();

                //if it takes too long to scrap all the data, set this to false
                bool scrapAllPages = true;

                if (scrapAllPages && numberOfPages > 1)
                {
                    for (int pageNumber = 2; pageNumber <= numberOfPages; pageNumber++)
                    {
                        HttpWebRequest newRequest = CreateWebRequest(UrlConstants.BankOfChina);
                        htmlDocument = newRequest.GetHtmlDocumentWithData(currency, pageNumber);
                        listOfRows.AddRange(htmlDocument.GetTableRows());
                    }
                }

                var records = listOfRows.ToListOfRecords();

                Extensions.Output($"{currency} currency record scraping finished... {records.Count} records scrapped");

                if (records.Count > 0)
                {
                    fileOutput.Create(records);
                }
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

        private static HttpWebResponse GetInitialResponse()
        {
            HttpWebRequest request = CreateWebRequest(UrlConstants.BankOfChina);
            return (HttpWebResponse)request.GetResponse();
        }

        private static HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest request;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            catch (UriFormatException)
            {
                request = null;
            }

            if (request == null)
            {
                throw new ApplicationException("Invalid URL: " + url);
            }

            return request;
        }
    }
}
