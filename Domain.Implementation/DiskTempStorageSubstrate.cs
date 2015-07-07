using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    public class DiskTempStorageSubstrate : ITempStorageSubstrate
    {
        private string _storagePath;

        public static ITempStorageSubstrate Create(string storagePath)
        {
            return new DiskTempStorageSubstrate(storagePath);
        }

        private DiskTempStorageSubstrate(string storagePath)
        {
            _storagePath = storagePath;
        }

        public void StoreData(string fileName, string clearTextData)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Invalid filename supplied to StoreData");

            string filePath = Path.Combine(_storagePath, fileName).ToString();
            File.WriteAllText(filePath, clearTextData);
        }

        public string RetrieveData(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Invalid filename supplied to RetrieveData");

            string filePath = Path.Combine(_storagePath, fileName).ToString();
            return File.ReadAllText(filePath);
        }

        public void EraseData(string fileName)
        {            
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Invalid filename supplied to EraseData");

            string filePath = Path.Combine(_storagePath, fileName).ToString();
            var contentLength = RetrieveData(fileName).Length;
            var random = new Random();
            var byteArray = new byte[contentLength];
            for (int numberOfPasses = 0; numberOfPasses < 7; numberOfPasses++)
            {
                random.NextBytes(byteArray);
                File.WriteAllBytes(filePath, byteArray);
            }
            File.Delete(fileName);
        }

        public string GetPath(string fileName)
        {
            return Path.Combine(_storagePath, fileName);
        }
    }
}
