using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.TestMocks
{
    public class MockTempStorageSubstrate : ITempStorageSubstrate
    {
        private Dictionary<string, string> _dataStore;

        public MockTempStorageSubstrate()
        {
            _dataStore = new Dictionary<string, string>();
        }

        public void StoreData(string fileName, string clearTextData)
        {
            _dataStore[fileName.ToLower()] = clearTextData;   
        }

        public string RetrieveData(string fileName)
        {
            return _dataStore.ContainsKey(fileName.ToLower()) ? _dataStore[fileName.ToLower()] : null;
        }

        public void EraseData(string fileName)
        {
            if (_dataStore.ContainsKey(fileName.ToLower()))
                _dataStore[fileName.ToLower()] = "";
        }
    }
}
