using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.TestMocks
{
    public class MockEnvironmentScanner : IEnvironmentScanner
    {
        private List<ICryptographicServiceProvider> _cryptoProviderList;
        private List<IStorageSubstrate> _storageSubstrateList;

        public MockEnvironmentScanner(List<ICryptographicServiceProvider> cryptoProviderList,
            List<IStorageSubstrate> storageSubstrateList)
        {
            _cryptoProviderList = cryptoProviderList;
            _storageSubstrateList = storageSubstrateList;
        }

        public List<ICryptographicServiceProvider> GetCryptographicProviders()
        {
            return _cryptoProviderList;
        }

        public List<IStorageSubstrate> GetStorageSubstrates()
        {
            return _storageSubstrateList;
        }


        public ITempStorageSubstrate GetTemporaryStorageSubstrate()
        {
            return new MockTempStorageSubstrate();
        }

        public string GetEditor()
        {
            return "notepad.exe";
        }
    }
}
