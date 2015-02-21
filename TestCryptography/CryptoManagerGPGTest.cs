using System;
using Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TestUtilities;
using System.Net;
using Cryptography.GPG;
using Mercurio.Domain;

namespace TestCryptography
{
    [TestClass]
    public class CryptoManagerGPGTest
    {
        private const string hermesPublicKeyID = "6C628193";
        private const string hermesPassphrase = @"Our technology has been TurneD AGAINST US :(";

        [TestMethod]
        public void GPGCryptoManagerTest()
        {
            CryptoManagerConfiguration configuration = TestUtilities.TestConfig.Create("mercurio");
            CryptoManagerFactory.Register(CryptoType.GPG.ToString(), typeof(CrypographicServiceProviderGPG));
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoType.GPG.ToString());
            string publicKey = cryptoManager.GetPublicKey(string.Empty);
            Assert.IsTrue(publicKey.Length != 0);

            string publicKey2 = cryptoManager.GetPublicKey(hermesPublicKeyID);
            Assert.IsTrue(publicKey2.Length != 0);
            Assert.IsTrue(publicKey2 == publicKey);

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
