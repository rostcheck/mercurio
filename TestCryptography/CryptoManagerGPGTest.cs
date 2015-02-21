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
        public void GPGCryptoManager_Test_GetPublicKey()
        {
            CryptoManagerConfiguration configuration = TestUtilities.TestConfig.Create("mercurio");
            CryptoManagerFactory.Register(CryptoType.GPG.ToString(), typeof(CrypographicServiceProviderGPG));
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoType.GPG.ToString());
            string publicKey = cryptoManager.GetPublicKey(string.Empty);
            Assert.IsTrue(publicKey.Length != 0);

            string publicKey2 = cryptoManager.GetPublicKey(null);
            Assert.IsTrue(publicKey2 == publicKey);

            string publicKey3 = cryptoManager.GetPublicKey(hermesPublicKeyID);
            Assert.IsTrue(publicKey3 == publicKey);
        }

        [TestMethod]
        [ExpectedException(typeof(MercurioException))]
        public void GPGCryptoManager_test_GetPublicKey_Failure()
        {
            CryptoManagerConfiguration configuration = TestUtilities.TestConfig.Create("mercurio");
            CryptoManagerFactory.Register(CryptoType.GPG.ToString(), typeof(CrypographicServiceProviderGPG));
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoType.GPG.ToString());
            string publicKey = cryptoManager.GetPublicKey("bad-key-id");

        }

        [TestMethod]
        public void GPGCryptoManager_Test_ValidateCredential()
        {
            CryptoManagerConfiguration configuration = TestUtilities.TestConfig.Create("mercurio");
            CryptoManagerFactory.Register(CryptoType.GPG.ToString(), typeof(CrypographicServiceProviderGPG));
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoType.GPG.ToString());

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
