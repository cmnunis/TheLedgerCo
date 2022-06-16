namespace TheLedgerCo.Models
{
    public class Payment
    {
        public Borrower Borrower { get; set; }
        public decimal LumpSumAmount { get; set; }
        public decimal NumberOfEquatedMonthlyInstallments { get; set; }

        public Payment(Borrower borrower, decimal lumpSumAmount, decimal numberOfEquatedMonthlyInstallments)
        {
            Borrower = borrower;
            LumpSumAmount = lumpSumAmount;
            NumberOfEquatedMonthlyInstallments = numberOfEquatedMonthlyInstallments;
        }
    }
}
