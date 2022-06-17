using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TheLedgerCo.Models;
using TheLedgerCo.Services;
using Xunit;

namespace TheLedgerCoTests.Services
{
    public class LoanRepaymentInfoSummaryServiceTests
    {
        private readonly Mock<ICalculatorService> _mockCalculatorService;
        private readonly ILoanRepaymentInfoSummaryService _loanRepaymentInfoSummaryService;

        public LoanRepaymentInfoSummaryServiceTests()
        {
            _mockCalculatorService = new Mock<ICalculatorService>();
            _loanRepaymentInfoSummaryService = new LoanRepaymentInfoSummaryService(_mockCalculatorService.Object);
        }

        [Fact]
        public void CalculatorServiceIsNull_ThrowArgumentNullException()
        {
            var sut = Assert.Throws<ArgumentNullException>(() => new LoanRepaymentInfoSummaryService(null));
            sut.ParamName.Should().Be("calculatorService");
        }

        [Fact]
        public void GenerateLoanRepaymentInfoSummary_HasLoanBalanceAndPaymentInput_ReturnsOutput()
        {
            var borrower = new Borrower("ANZ", "Collin");
            var loanRepaymentInfo = new LoanRepaymentInfo { Borrower = borrower, TotalAmountPaidToDate = 1000, MonthlyInstallmentsRemaining = 55 };

            var loans = new List<Loan> { new Loan(borrower, 10000, 5, 4) };
            var balances = new List<Balance> { new Balance(borrower, 5) };
            var payments = new List<Payment>();

            _mockCalculatorService.Setup(x => x.GetLoanRepaymentInfo(It.IsAny<Balance>(), It.IsAny<Loan>(), null)).Returns(loanRepaymentInfo);
            var sut = _loanRepaymentInfoSummaryService.GenerateLoanRepaymentInfoSummary(loans, payments, balances);

            sut.Should().NotBeNull();
            sut.Should().OnlyHaveUniqueItems();

            foreach (var instance in sut)
            {
                instance.Borrower.BorrowerName.Should().Be("Collin");
                instance.Borrower.BankName.Should().Be("ANZ");
                instance.MonthlyInstallmentsRemaining.Should().Be(55);
                instance.TotalAmountPaidToDate.Should().Be(1000);
            }
        }

        [Fact]
        public void GenerateLoanRepaymentInfoSummary_HasLoanAndBalanceInput_ReturnsOutput()
        {
            var borrower = new Borrower("ANZ", "Collin");
            var loanRepaymentInfo = new LoanRepaymentInfo { Borrower = borrower, TotalAmountPaidToDate = 3652, MonthlyInstallmentsRemaining = 4 };

            var loans = new List<Loan> { new Loan(borrower, 5000, 1, 6) };
            var balances = new List<Balance> { new Balance(borrower, 6) };
            var payments = new List<Payment> { new Payment(borrower, 1000, 5)};

            _mockCalculatorService.Setup(x => x.GetLoanRepaymentInfo(It.IsAny<Balance>(), It.IsAny<Loan>(), It.IsAny<Payment>())).Returns(loanRepaymentInfo);
            var sut = _loanRepaymentInfoSummaryService.GenerateLoanRepaymentInfoSummary(loans, payments, balances);

            sut.Should().NotBeNull();
            sut.Should().OnlyHaveUniqueItems();

            foreach (var instance in sut)
            {
                instance.Borrower.BorrowerName.Should().Be("Collin");
                instance.Borrower.BankName.Should().Be("ANZ");
                instance.MonthlyInstallmentsRemaining.Should().Be(4);
                instance.TotalAmountPaidToDate.Should().Be(3652);
            }
        }

    }
}
