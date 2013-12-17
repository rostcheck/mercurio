﻿using System;
using Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TestUtils;

namespace TestEntities
{
    [TestClass]
    public class CryptoManagerTest
    {
        [TestMethod]
        public void GPGCryptoManagerTest()
        {
            Dictionary<ConfigurationKeyEnum, string> configuration = TestUtils.TestConfiguration1.Create();
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoManagerType.GPGCryptoManager, configuration);
            string publicKey = cryptoManager.GetPublicKey(string.Empty);
            Assert.IsTrue(publicKey.Length != 0);
        }
    }
}
