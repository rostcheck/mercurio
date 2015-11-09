using System;
using Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TestUtilities;
using System.Net;
using Cryptography.GPG;
using Mercurio.Domain;
using Starksoft.Cryptography.OpenPGP;
using Mercurio.Domain.Implementation;

namespace TestCryptography
{
    [TestClass]
    public class CryptoManagerGPGTest
    {
        [TestMethod]
        public void GPGCryptoManager_Test_GetPublicKey()
        {
            CryptoManagerConfiguration configuration = PrepareTest("mercurio");
            CryptoManagerFactory.Register(CryptoType.GPG.ToString(), typeof(CrypographicServiceProviderGPG));
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoType.GPG.ToString(), osAbstractor, configuration);
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
            CryptoManagerConfiguration configuration = PrepareTest("mercurio");
            CryptoManagerFactory.Register(CryptoType.GPG.ToString(), typeof(CrypographicServiceProviderGPG));
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoType.GPG.ToString(), osAbstractor, configuration);
            string publicKey = cryptoManager.GetPublicKey("bad-key-id");
        }

        [TestMethod]
        public void GPGCryptoManager_Test_ValidateCredential()
        {
            CryptoManagerConfiguration configuration = PrepareTest("mercurio");
            CryptoManagerFactory.Register(CryptoType.GPG.ToString(), typeof(CrypographicServiceProviderGPG));
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoType.GPG.ToString(), osAbstractor, configuration);

            NetworkCredential goodCredential = new NetworkCredential(CryptoTestConstants.HermesPublicKeyID, CryptoTestConstants.HermesPassphrase);
            NetworkCredential badCredential = new NetworkCredential(CryptoTestConstants.HermesPublicKeyID, "Not the correct passphrase");
            Assert.IsTrue(cryptoManager.ValidateCredential(goodCredential));
            Assert.IsFalse(cryptoManager.ValidateCredential(badCredential));
        }

        [TestMethod]
        public void GPGCryptoManager_Test_SetCredential_allows_setting_to_null()
        {
            CryptoManagerConfiguration configuration = PrepareTest("mercurio");
            CryptoManagerFactory.Register(CryptoType.GPG.ToString(), typeof(CrypographicServiceProviderGPG));
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoType.GPG.ToString(), osAbstractor, configuration);

            cryptoManager.SetCredential(null);
        }

        [TestMethod]
        [ExpectedException(typeof(GnuPGException))]
        public void GPGCryptoManager_Test_SetCredential_throws_when_identity_does_not_exist()
        {
            CryptoManagerConfiguration configuration = PrepareTest("mercurio");
            CryptoManagerFactory.Register(CryptoType.GPG.ToString(), typeof(CrypographicServiceProviderGPG));
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoType.GPG.ToString(), osAbstractor, configuration);

            NetworkCredential badCredential = new NetworkCredential("invalid-identity", "Not the correct passphrase");
            cryptoManager.SetCredential(badCredential);
        }

        private CryptoManagerConfiguration PrepareTest(string userName)
        {
            TestUtils.SetupUserDir(userName);
            TestUtils.SwitchUser(null, userName);
            return TestConfig.Create(userName);
        }
    }
}
