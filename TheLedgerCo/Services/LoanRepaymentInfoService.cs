using Microsoft.Extensions.Configuration;
using TheLedgerCo.Models;
using TheLedgerCo.Constants;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheLedgerCo.Services
{
    public interface ILoanRepaymentInfoService
    {
        Task GenerateLoanRepaymentInfoAsync(string[] args);
    }

    public class LoanRepaymentInfoService : ILoanRepaymentInfoService
    {
        private readonly ICommandToLedgerObjectConverterService _commandToLedgerObjectConverter;
        private readonly IConfiguration _configuration;
        private readonly IFileReaderService _fileReaderService;
        private readonly ICalculatorService _calculatorService;

        public LoanRepaymentInfoService(ICommandToLedgerObjectConverterService commandToLedgerObjectConverter, IConfiguration configuration, IFileReaderService fileReaderService, ICalculatorService calculatorService)
        {
            _commandToLedgerObjectConverter = commandToLedgerObjectConverter ?? throw new ArgumentNullException(nameof(commandToLedgerObjectConverter));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _fileReaderService = fileReaderService ?? throw new ArgumentNullException(nameof(fileReaderService));
            _calculatorService = calculatorService ?? throw new ArgumentNullException(nameof(calculatorService));
        }

        public async Task GenerateLoanRepaymentInfoAsync(string[] args)
        {
            var files = _fileReaderService.GetAvailableInputFiles(args[0]);

            if (files == null)
            {
                Console.WriteLine("No files were found in the specified directory.");
                return;
            }

            for (int i = 0; i < files.Length; i++)
            {
                var loans = new List<Loan>();
                var payments = new List<Payment>();
                var balanceQueries = new List<Balance>();

                var fileContents = await _fileReaderService.GetFileContentsAsync(files[i]);

                ReadFileContents(loans, payments, balanceQueries, fileContents);

                Console.WriteLine($"\r\nOUTPUT for File {i + 1}");
                Console.WriteLine("-------------------");
                var loanRepaymentInfoSummary = GenerateLoanRepaymentInfoSummary(loans, payments, balanceQueries);
                OutputLoanRepaymentInfoSummary(loanRepaymentInfoSummary);
            }
        }

        private List<LoanRepaymentInfo> GenerateLoanRepaymentInfoSummary(List<Loan> loans, List<Payment> payments, List<Balance> balanceQueries)
        {
            var loanRepaymentInfoList = new List<LoanRepaymentInfo>();

            var balanceQueriesGroupedByBorrower = balanceQueries.GroupBy(x => new { x.Borrower.BorrowerName, x.Borrower.BankName })
                                                            .Select(y => new { y.Key.BorrowerName, y.Key.BankName })
                                                            .ToList();

            foreach (var key in balanceQueriesGroupedByBorrower)
            {
                var borrowerBalanceQueries = balanceQueries.Where(x => x.Borrower.BorrowerName.Equals(key.BorrowerName)
                                                                       && x.Borrower.BankName.Equals(key.BankName));

                foreach (var balanceQuery in borrowerBalanceQueries)
                {
                    var borrowerLoanRecords = loans.Where(x => x.Borrower.BorrowerName.Equals(key.BorrowerName)
                                                                       && x.Borrower.BankName.Equals(key.BankName));

                    foreach (var loan in borrowerLoanRecords)
                    {
                        var lumpSumPaymentsMadeByBorrower = payments.Where(x => x.Borrower.BorrowerName.Equals(key.BorrowerName)
                                                                       && x.Borrower.BankName.Equals(key.BankName));

                        if (lumpSumPaymentsMadeByBorrower.Count() > 0)
                        {
                            foreach (var lumpSumPayment in lumpSumPaymentsMadeByBorrower)
                            {
                                var loanRepaymentInfo = _calculatorService.GetLoanRepaymentInfo(balanceQuery, loan, payment: lumpSumPayment);
                                loanRepaymentInfoList.Add(loanRepaymentInfo);
                            }
                        }
                        else
                        {
                            var loanRepaymentInfo = _calculatorService.GetLoanRepaymentInfo(balanceQuery, loan);
                            loanRepaymentInfoList.Add(loanRepaymentInfo);
                        }
                    }
                }
            }

            return loanRepaymentInfoList;
        }

        private string[] GetCommandQuery(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                throw new ArgumentNullException(nameof(line));

            return line.Split(" ");
        }

        private void BuildDataListFromObjects(string[] commandStringArray, List<Loan> loans, List<Payment> payments, List<Balance> balanceQueries)
        {
            if (commandStringArray[0].Equals(Commands.Loan, StringComparison.OrdinalIgnoreCase))
            {
                var loan = _commandToLedgerObjectConverter.BuildLoanObject(commandStringArray);
                loans.Add(loan);
            }
            else if (commandStringArray[0].Equals(Commands.Payment, StringComparison.OrdinalIgnoreCase))
            {
                var payment = _commandToLedgerObjectConverter.BuildPaymentObject(commandStringArray);
                payments.Add(payment);
            }
            else if (commandStringArray[0].Equals(Commands.Balance, StringComparison.OrdinalIgnoreCase))
            {
                var balance = _commandToLedgerObjectConverter.BuildBalanceObject(commandStringArray);
                balanceQueries.Add(balance);
            }
        }

        private void OutputLoanRepaymentInfoSummary(List<LoanRepaymentInfo> loanRepaymentInfoSummary)
        {
            for (int i = 0; i < loanRepaymentInfoSummary.Count; i++)
            {
                var loanRepaymentInfo = loanRepaymentInfoSummary[i];
                var output = $"{loanRepaymentInfo.Borrower.BankName} {loanRepaymentInfo.Borrower.BorrowerName} {loanRepaymentInfo.TotalAmountPaidToDate} {loanRepaymentInfo.MonthlyInstallmentsRemaining}";

                if (loanRepaymentInfoSummary[i] == loanRepaymentInfoSummary.Last())
                    Console.WriteLine($"{output}\n");
                else
                    Console.WriteLine($"{output}");
            }
        }

        private void ReadFileContents(List<Loan> loans, List<Payment> payments, List<Balance> balanceQueries, string[] fileContents)
        {
            if (fileContents == null)
            {
                return;
            }

            foreach (var line in fileContents)
            {
                var commandStringArray = GetCommandQuery(line);

                BuildDataListFromObjects(commandStringArray, loans, payments, balanceQueries);
            }
        }
    }
}
