using System;
using TheLedgerCo.Services;
using Xunit;

namespace TheLedgerCoTests.Services
{
    public class FileReaderServiceTests
    {
        private readonly FileReaderService _fileReaderService;

        public FileReaderServiceTests()
        {
            _fileReaderService = new FileReaderService();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void FilePathIsNullOrWhitespaceThrowsException(string value)
        {
            var sut = Assert.Throws<ArgumentNullException>(() => _fileReaderService.GetAvailableInputFiles(value));
            Assert.Equal("path", sut.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void FileNameIsNullOrWhitespaceThrowsException(string value)
        {
            var sut = Assert.ThrowsAsync<ArgumentNullException>(async () => await _fileReaderService.GetFileContentsAsync(value));
            Assert.Equal("fileName", sut.Result.ParamName);
        }
    }
}
