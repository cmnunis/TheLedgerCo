using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using TheLedgerCo.Constants;
using TheLedgerCo.Services;

namespace TheLedgerCo
{
    static class Program
    {
        public static IConfigurationRoot Configuration { get; private set; }

        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }


        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, configuration) =>
            {
                configuration.Sources.Clear();
                configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                IConfigurationRoot configurationRoot = configuration.Build();
                Configuration = configurationRoot;

            }).ConfigureServices((services) =>
            {
                var section = Configuration?.GetSection(nameof(ConfigurationSettings));
                var configSettings = section.Get<ConfigurationSettings>();

                services.AddSingleton(configSettings);
                services.AddScoped<ICommandToLedgerObjectConverterService, CommandToLedgerObjectConverterService>();
                services.AddScoped<ICalculatorService, CalculatorService>();
                services.AddScoped<IFileReaderService, FileReaderService>();
                services.AddScoped<ILoanRepaymentInfoService, LoanRepaymentInfoService>();
                services.AddHostedService<ConsoleService>();
            });
    }
}