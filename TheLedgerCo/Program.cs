using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using TheLedgerCo.Services;

namespace TheLedgerCo
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }


        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((services) =>
            {
                services.AddSingleton(args);
                services.AddScoped<ICommandToLedgerObjectConverterService, CommandToLedgerObjectConverterService>();
                services.AddScoped<ICalculatorService, CalculatorService>();
                services.AddScoped<IFileReaderService, FileReaderService>();
                services.AddScoped<ILoanRepaymentInfoSummaryService, LoanRepaymentInfoSummaryService>();
                services.AddScoped<ILoanRepaymentInfoService, LoanRepaymentInfoService>();
                services.AddHostedService<ConsoleService>();
            });
    }
}