using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.TestMocks
{
    public class MockCryptographicServiceProvider : ICryptographicServiceProvider
    {
        private CryptoManagerConfiguration _cryptoManagerConfiguration;

        public string GetProviderType()
        {
            return "MockCrypto";
        }

        public bool IsInstalled(IOSAbstractor abstractor)
        {
            return true;
        }


        public CryptoManagerConfiguration GetConfiguration()
        {
            return _cryptoManagerConfiguration;
        }

        public ICryptoManager CreateManager(CryptoManagerConfiguration configuration)
        {
            var cryptoManager = new MockCryptoManager();
            //cryptoManager.SetConfiguration(configuration);
            _cryptoManagerConfiguration = configuration;
            return cryptoManager;
        }
    }
}
