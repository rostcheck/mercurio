using System;
using Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TestUtilities;

namespace TestEntities
{
    [TestClass]
    public class CryptoManagerTest
    {
        [TestMethod]
        public void GPGCryptoManagerTest()
        {
            Dictionary<ConfigurationKeyEnum, string> configuration = TestUtilities.TestConfig.Create();
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoManagerType.GPGCryptoManager, configuration);
            string publicKey = cryptoManager.GetPublicKey(string.Empty);
            Assert.IsTrue(publicKey.Length != 0);
        }
    }
}
