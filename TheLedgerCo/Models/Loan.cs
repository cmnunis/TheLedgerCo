using System;

namespace TheLedgerCo.Models
{
    public class Loan
    {
        public Borrower Borrower { get; set; }
        public decimal Principal { get; set; }
        public decimal InterestRate { get; set; }
        public int NumberOfYears { get; set; }
        public decimal CalculatedInterest { get { return Principal * NumberOfYears * (InterestRate / 100); } }
        public decimal EquatedMonthlyInstallment { get { return Math.Ceiling((Principal + CalculatedInterest) / RepaymentTermInMonths); } }
        public decimal TotalLoanAmountIncludingInterest { get { return Principal + CalculatedInterest; } }
        public int RepaymentTermInMonths { get { return NumberOfYears * 12; } }
        

        public Loan(Borrower borrower, decimal principal, int numberOfYears, decimal interestRate)
        {
            Borrower = borrower;
            Principal = principal;
            NumberOfYears = numberOfYears;
            InterestRate = interestRate;    
        }
    }
}
