using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheLedgerCo.Models;

namespace TheLedgerCo.Services
{
    public interface ILoanRepaymentInfoSummaryService
    {
        List<LoanRepaymentInfo> GenerateLoanRepaymentInfoSummary(List<Loan> loans, List<Payment> payments, List<Balance> balanceQueries);
        string OutputLoanRepaymentInfoSummary(List<LoanRepaymentInfo> loanRepaymentInfoSummary);
    }

    public class LoanRepaymentInfoSummaryService : ILoanRepaymentInfoSummaryService
    {
        private readonly ICalculatorService _calculatorService;

        public LoanRepaymentInfoSummaryService(ICalculatorService calculatorService)
        {
            _calculatorService = calculatorService ?? throw new ArgumentNullException(nameof(calculatorService));
        }

        public List<LoanRepaymentInfo> GenerateLoanRepaymentInfoSummary(List<Loan> loans, List<Payment> payments, List<Balance> balanceQueries)
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

        public string OutputLoanRepaymentInfoSummary(List<LoanRepaymentInfo> loanRepaymentInfoSummary)
        {
            var output = string.Empty;

            for (int i = 0; i < loanRepaymentInfoSummary.Count; i++)
            {
                var loanRepaymentInfo = loanRepaymentInfoSummary[i];
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(loanRepaymentInfo.Borrower.BankName);
                stringBuilder.Append(" ");
                stringBuilder.Append(loanRepaymentInfo.TotalAmountPaidToDate);
                stringBuilder.Append(" ");
                stringBuilder.Append(loanRepaymentInfo.Borrower.BorrowerName);
                stringBuilder.Append(" ");
                stringBuilder.Append(loanRepaymentInfo.MonthlyInstallmentsRemaining);
                stringBuilder.Append(loanRepaymentInfoSummary[i] == loanRepaymentInfoSummary.Last() ? "\n" : string.Empty);

                output += $"{stringBuilder}\n";
            }

            return output;
        }
    }
}
