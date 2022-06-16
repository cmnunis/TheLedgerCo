using System;
using System.IO;
using System.Threading.Tasks;

namespace TheLedgerCo.Services
{
    public interface IFileReaderService
    {
        string[] GetAvailableInputFiles(string path);
        Task<string[]> GetFileContentsAsync(string fileName);
    }
    public class FileReaderService : IFileReaderService
    {
        public string[] GetAvailableInputFiles(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            return Directory.GetFiles(path, "*.txt");
        }

        public async Task<string[]> GetFileContentsAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            return await File.ReadAllLinesAsync(fileName);
        }
    }
}
