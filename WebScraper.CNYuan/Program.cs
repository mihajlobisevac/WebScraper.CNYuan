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
            var serviceProvider = RegisterServices(args);
            var configuration = serviceProvider.GetService<IConfiguration>();

            Scrapper.BeginScrapping(configuration);

            serviceProvider.Dispose();
            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();

                    var envName = hostingContext.HostingEnvironment.EnvironmentName;

                    configuration
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{envName}.json", true, true);

                    IConfigurationRoot configurationRoot = configuration.Build();
                });

        static IConfiguration SetupConfiguration(string[] args)
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
        }

        static ServiceProvider RegisterServices(string[] args)
        {
            var configuration = SetupConfiguration(args);

            return new ServiceCollection()
                .AddSingleton(configuration)
                .BuildServiceProvider();
        }
    }
}
