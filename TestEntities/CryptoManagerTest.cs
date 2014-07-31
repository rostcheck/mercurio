using System;
using Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TestUtilities;
using System.Net;

namespace TestEntities
{
    [TestClass]
    public class CryptoManagerTest
    {
        private const string hermesPublicKeyID = "6C628193";
        private const string hermesPassphrase = @"Our technology has been TurneD AGAINST US :(";

        [TestMethod]
        public void GPGCryptoManagerTest()
        {
            //PrepareTest("mercurio");
            Dictionary<ConfigurationKeyEnum, string> configuration = TestUtilities.TestConfig.Create("mercurio");
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoManagerType.GPGCryptoManager, configuration);
            string publicKey = cryptoManager.GetPublicKey(string.Empty);
            Assert.IsTrue(publicKey.Length != 0);

            NetworkCredential goodCredential = new NetworkCredential(hermesPublicKeyID, hermesPassphrase);
            NetworkCredential badCredential = new NetworkCredential(hermesPublicKeyID, "Not the correct passphrase");
            Assert.IsTrue(cryptoManager.ValidateCredential(goodCredential));
            Assert.IsFalse(cryptoManager.ValidateCredential(badCredential));
        }

        private void PrepareTest(string userName)
        {
            TestUtils.SetupUserDir(userName);
            TestUtils.SwitchUser(null, userName);
        }
    }
}
