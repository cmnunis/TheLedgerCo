using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheLedgerCo.Services
{
    public class ConsoleService : IHostedService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILoanRepaymentInfoService _loanRepaymentInfoService;

        public ConsoleService(IHostApplicationLifetime applicationLifetime, ILoanRepaymentInfoService loanRepaymentInfoService)
        {
            _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            _loanRepaymentInfoService = loanRepaymentInfoService ?? throw new ArgumentNullException(nameof(loanRepaymentInfoService));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            _applicationLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                       await _loanRepaymentInfoService.GenerateLoanRepaymentInfoAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    finally
                    {

                        _applicationLifetime.StopApplication();
                    }
                });
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
