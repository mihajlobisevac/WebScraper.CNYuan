using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;
using WebScraper.CNYuan.Services;

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

            Scrapper.BeginScrapping(fileOutput);

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
