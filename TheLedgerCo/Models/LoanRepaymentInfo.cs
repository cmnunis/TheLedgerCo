namespace TheLedgerCo.Models
{
    public class LoanRepaymentInfo
    { 
        public Borrower Borrower { get; set; }
        public decimal TotalAmountPaidToDate { get; set; }
        public decimal MonthlyInstallmentsRemaining { get; set; }
    }
}
