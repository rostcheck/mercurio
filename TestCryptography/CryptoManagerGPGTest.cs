using System;
using Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TestUtilities;
using System.Net;
using Cryptography.GPG;
using Mercurio.Domain;
using Starksoft.Cryptography.OpenPGP;

namespace TestCryptography
{
    [TestClass]
    public class CryptoManagerGPGTest
    {
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

            string publicKey3 = cryptoManager.GetPublicKey(CryptoTestConstants.HermesPublicKeyID);
            Assert.IsTrue(publicKey3 == publicKey);
        }

        [TestMethod]
        [ExpectedException(typeof(MercurioException))]
        public void GPGCryptoManager_Test_GetPublicKey_Failure()
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

            NetworkCredential goodCredential = new NetworkCredential(CryptoTestConstants.HermesPublicKeyID, CryptoTestConstants.HermesPassphrase);
            NetworkCredential badCredential = new NetworkCredential(CryptoTestConstants.HermesPublicKeyID, "Not the correct passphrase");
            Assert.IsTrue(cryptoManager.ValidateCredential(goodCredential));
            Assert.IsFalse(cryptoManager.ValidateCredential(badCredential));
        }

        [TestMethod]
        public void GPGCryptoManager_Test_SetCredential_allows_setting_to_null()
        {
            CryptoManagerConfiguration configuration = TestUtilities.TestConfig.Create("mercurio");
            CryptoManagerFactory.Register(CryptoType.GPG.ToString(), typeof(CrypographicServiceProviderGPG));
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoType.GPG.ToString());

            cryptoManager.SetCredential(null);
        }

        [TestMethod]
        [ExpectedException(typeof(GnuPGException))]
        public void GPGCryptoManager_Test_SetCredential_throws_when_identity_does_not_exist()
        {
            CryptoManagerConfiguration configuration = TestUtilities.TestConfig.Create("mercurio");
            CryptoManagerFactory.Register(CryptoType.GPG.ToString(), typeof(CrypographicServiceProviderGPG));
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoType.GPG.ToString());

            NetworkCredential badCredential = new NetworkCredential("invalid-identity", "Not the correct passphrase");
            cryptoManager.SetCredential(badCredential);
        }

        private void PrepareTest(string userName)
        {
            TestUtils.SetupUserDir(userName);
            TestUtils.SwitchUser(null, userName);
        }
    }
}
