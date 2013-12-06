using System;
using Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TestEntities
{
    [TestClass]
    public class CryptoManagerTest
    {
        [TestMethod]
        public void GPGCryptoManagerTest()
        {
            Dictionary<ConfigurationKeyEnum, string> configuration = TestConfiguration.Create();
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoManagerType.GPGCryptoManager, configuration);
            string publicKey = cryptoManager.GetPublicKey(string.Empty);
            Assert.IsTrue(publicKey.Length != 0);
        }
    }
}
