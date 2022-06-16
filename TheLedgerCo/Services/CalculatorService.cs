using System;
using TheLedgerCo.Models;

namespace TheLedgerCo.Services
{
    public interface ICalculatorService
    {
        LoanRepaymentInfo GetLoanRepaymentInfo(Balance balanceQuery, Loan loanInfo, Payment payment = null);
    }

    public class CalculatorService : ICalculatorService
    {
        public LoanRepaymentInfo GetLoanRepaymentInfo(Balance balanceQuery, Loan loanInfo, Payment payment = null)
        {
            var loanRepaymentInfo = new LoanRepaymentInfo { Borrower = balanceQuery.Borrower };

            if (payment != null && (balanceQuery.NumberOfEquatedMonthlyInstallments >= payment.NumberOfEquatedMonthlyInstallments))
            {
                var totalAmountRepaidToDate = (loanInfo.EquatedMonthlyInstallment * balanceQuery.NumberOfEquatedMonthlyInstallments) + payment.LumpSumAmount;
                var balanceRemainingOnLoan = (loanInfo.TotalLoanAmountIncludingInterest - totalAmountRepaidToDate);

                loanRepaymentInfo.TotalAmountPaidToDate = totalAmountRepaidToDate;
                loanRepaymentInfo.MonthlyInstallmentsRemaining = Math.Ceiling(balanceRemainingOnLoan / loanInfo.EquatedMonthlyInstallment);

                return loanRepaymentInfo;
            }
            else
            {
                loanRepaymentInfo.TotalAmountPaidToDate = Math.Ceiling(loanInfo.EquatedMonthlyInstallment * balanceQuery.NumberOfEquatedMonthlyInstallments);
                loanRepaymentInfo.MonthlyInstallmentsRemaining = Math.Abs(loanInfo.RepaymentTermInMonths - balanceQuery.NumberOfEquatedMonthlyInstallments);

                return loanRepaymentInfo;
            }
        }
    }
}
