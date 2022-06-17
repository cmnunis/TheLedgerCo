using System;
using Xunit;
using Moq;
using FluentAssertions;
using TheLedgerCo.Services;
using TheLedgerCo.Models;
using System.Collections.Generic;

namespace TheLedgerCoTests.Services
{
    public class LoanRepaymentInfoServiceTests
    {
        private readonly Mock<ICommandToLedgerObjectConverterService> _mockCommandToLedgerObjectConverter;
        private readonly Mock<IFileReaderService> _mockFileReaderService;
        private readonly Mock<ILoanRepaymentInfoSummaryService> _mockLoanRepaymentInfoSummaryService;
        private readonly ILoanRepaymentInfoService _loanRepaymentInfoService;

        public LoanRepaymentInfoServiceTests()
        {
            _mockCommandToLedgerObjectConverter = new Mock<ICommandToLedgerObjectConverterService>();
            _mockFileReaderService = new Mock<IFileReaderService>();
            _mockLoanRepaymentInfoSummaryService = new Mock<ILoanRepaymentInfoSummaryService>();
            _loanRepaymentInfoService = new LoanRepaymentInfoService(_mockCommandToLedgerObjectConverter.Object,
                _mockFileReaderService.Object, _mockLoanRepaymentInfoSummaryService.Object);
        }

        [Fact]
        public void CommandToLedgerObjectConverterIsNull_ThrowsArgumentNullException()
        {
            var sut = Assert.Throws<ArgumentNullException>(() => new LoanRepaymentInfoService(null,
                _mockFileReaderService.Object, _mockLoanRepaymentInfoSummaryService.Object));

            sut.ParamName.Should().Be("commandToLedgerObjectConverter");
        }

        [Fact]
        public void FileReaderServiceIsNull_ThrowsArgumentNullException()
        {
            var sut = Assert.Throws<ArgumentNullException>(() => new LoanRepaymentInfoService(_mockCommandToLedgerObjectConverter.Object,
                null, _mockLoanRepaymentInfoSummaryService.Object));

            sut.ParamName.Should().Be("fileReaderService");
        }

        [Fact]
        public void ILoanRepaymentInfoSummaryServiceIsNull_ThrowsArgumentNullException()
        {
            var sut = Assert.Throws<ArgumentNullException>(() => new LoanRepaymentInfoService(_mockCommandToLedgerObjectConverter.Object,
                _mockFileReaderService.Object, null));

            sut.ParamName.Should().Be("loanRepaymentInfoSummaryService");
        }

        [Fact]
        public void GenerateLoanRepaymentInfoAsync_ArgsIsNull_ThrowsArgumentNullException()
        {
            var sut = Assert.ThrowsAsync<ArgumentNullException>(async() => await _loanRepaymentInfoService.GenerateLoanRepaymentInfoAsync(null));

            sut.Result.ParamName.Should().Be("args");
        }

        [Fact]
        public async void GenerateLoanRepaymentInfoAsync_NoPaymentCommand_ArgsIsNotNull_ReturnsOutput()
        {
            var args = new string[] { "test/somefilePath" };
            var filesInPath = new string[] { "file1" };

            _mockFileReaderService.Setup(x => x.GetAvailableInputFiles("test/somefilePath")).Returns(filesInPath);

            var fileContents = new string[] { "LOAN ANZ Collin 10000 5 4", "BALANCE ANZ Collin 5" };
            _mockFileReaderService.Setup(x => x.GetFileContentsAsync(It.IsAny<string>()))
                                  .ReturnsAsync(fileContents);

            var borrower = new Borrower("ANZ", "Collin");
            var loan = new Loan(borrower, 10000, 5, 4);
            var balance = new Balance(borrower, 5);

            _mockCommandToLedgerObjectConverter.Setup(x => x.BuildLoanObject(It.IsAny<string[]>())).Returns(loan);
            _mockCommandToLedgerObjectConverter.Setup(x => x.BuildBalanceObject(It.IsAny<string[]>())).Returns(balance);

            await _loanRepaymentInfoService.GenerateLoanRepaymentInfoAsync(args);

            _mockLoanRepaymentInfoSummaryService.Setup(x => x.GenerateLoanRepaymentInfoSummary(It.IsAny<List<Loan>>(), null, It.IsAny<List<Balance>>()))
                                                .Returns(It.IsAny<List<LoanRepaymentInfo>>());

            _mockLoanRepaymentInfoSummaryService.Setup(x => x.OutputLoanRepaymentInfoSummary(It.IsAny<List<LoanRepaymentInfo>>()))
                                                .Verifiable();
        }

        [Fact]
        public async void GenerateLoanRepaymentInfoAsync_HasPaymentCommand_ArgsIsNotNull_ReturnsOutput()
        {
            var args = new string[] { "test/somefilePath" };
            var filesInPath = new string[] { "file1" };

            _mockFileReaderService.Setup(x => x.GetAvailableInputFiles("test/somefilePath")).Returns(filesInPath);

            var fileContents = new string[] { "LOAN ANZ Collin 5000 1 6", "PAYMENT ANZ Collin 1000 5", "BALANCE ANZ Collin 6" };
            _mockFileReaderService.Setup(x => x.GetFileContentsAsync(It.IsAny<string>()))
                                  .ReturnsAsync(fileContents);

            var borrower = new Borrower("ANZ", "Collin");
            var loan = new Loan(borrower, 10000, 5, 4);
            var payment = new Payment(borrower, 1000, 5);
            var balance = new Balance(borrower, 5);

            _mockCommandToLedgerObjectConverter.Setup(x => x.BuildLoanObject(It.IsAny<string[]>())).Returns(loan);
            _mockCommandToLedgerObjectConverter.Setup(x => x.BuildBalanceObject(It.IsAny<string[]>())).Returns(balance);
            _mockCommandToLedgerObjectConverter.Setup(x => x.BuildPaymentObject(It.IsAny<string[]>())).Returns(payment);

            await _loanRepaymentInfoService.GenerateLoanRepaymentInfoAsync(args);

            _mockLoanRepaymentInfoSummaryService.Setup(x => x.GenerateLoanRepaymentInfoSummary(It.IsAny<List<Loan>>(), It.IsAny<List<Payment>>(), It.IsAny<List<Balance>>()))
                                                .Returns(It.IsAny<List<LoanRepaymentInfo>>());

            _mockLoanRepaymentInfoSummaryService.Setup(x => x.OutputLoanRepaymentInfoSummary(It.IsAny<List<LoanRepaymentInfo>>()))
                                                .Verifiable();
        }
    }
}
