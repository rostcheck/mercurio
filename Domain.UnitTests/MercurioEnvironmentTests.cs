using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mercurio.Domain;
using System.Collections.Generic;
using System.Net;
using TestCryptography;

namespace Mercurio.Domain.UnitTests
{
    internal class MockCryptographicServiceProvider : ICryptographicServiceProvider
    {
        private CryptoManagerConfiguration _cryptoManagerConfiguration;

        public string GetProviderType()
        {
            return "MockCrypto";
        }

        public bool IsInstalled()
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

    internal class MockCryptoManager : ICryptoManager
    {
        private NetworkCredential _credential = null;

        public void SetCredential(NetworkCredential credential)
        {
            _credential = credential;
        }

        public void SetConfiguration(CryptoManagerConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public bool ValidateCredential(NetworkCredential credential)
        {
            if (_credential == null)
                return false;

            return true;
        }

        public string GetActiveIdentity()
        {
            return (_credential == null) ? string.Empty : _credential.UserName;
        }

        public System.IO.Stream Encrypt(System.IO.Stream messageStream, string identifier)
        {
            throw new NotImplementedException();
        }

        public string Encrypt(string message, string identifier)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream Decrypt(System.IO.Stream messageStream)
        {
            throw new NotImplementedException();
        }

        public string Sign(string message)
        {
            throw new NotImplementedException();
        }

        public bool Validate(string message)
        {
            throw new NotImplementedException();
        }

        public string GetPublicKey(string identifier)
        {
            throw new NotImplementedException();
        }

        public string ImportKey(string key)
        {
            throw new NotImplementedException();
        }

        public void SignKey(string identifier)
        {
            throw new NotImplementedException();
        }

        public string[] GetSignatures()
        {
            throw new NotImplementedException();
        }

        public string GetFingerprint(string identifier)
        {
            return "99288213";
        }

        public void DeleteKey(string identifier)
        {
            throw new NotImplementedException();
        }

        public bool HasPublicKey(string key)
        {
            throw new NotImplementedException();
        }

        public List<UserIdentity> GetAvailableIdentities()
        {
            var result = new List<UserIdentity>();
            result.Add(UserIdentity.Create(CryptoTestConstants.HermesPublicKeyID, "Test Identity", null, "Test User Identity", "MockCrypto"));
            return result;
        }

        public List<ContactIdentity> GetAvailableContactIdentities()
        {
            throw new NotImplementedException();
        }
        
        public void CreateKey(string identifier, NetworkCredential credential)
        {
            throw new NotImplementedException();
        }

        public string ManagerType
        {
            get { return "MockCryptoManager"; }
        }
    }

    internal class MockStorageSubstrate : IStorageSubstrate
    {
        private List<IContainer> _containers;

        public MockStorageSubstrate()
        {
            _containers = new List<IContainer>();
        }

        public string Name
        {
            get { return "MockStorageSubstate"; }
        }

        public IEnumerable<IContainer> GetAllContainers()
        {
            return new List<IContainer>(_containers);
        }

        public IContainer CreateContainer(string containerName, string keyFingerprint, ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicy)
        {
            var container = Container.Create(containerName, cryptoManager, retentionPolicy);
            _containers.Add(container);
            return container;
        }

        public IEnumerable<IContainer> GetAccessibleContainers(string identity, ICryptoManager cryptoManager)
        {
            throw new NotImplementedException();
        }
    }

    internal class MockEnvironmentScanner : IEnvironmentScanner
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
    }

    [TestClass]
    public class MercurioEnvironmentTests
    {
        private List<ICryptographicServiceProvider> _cryptoProviders;
        private List<IStorageSubstrate> _storageSubstrates;

        [TestInitialize]
        public void ContainerTests_Initialize()
        {
            this._cryptoProviders = new List<ICryptographicServiceProvider>();
            this._cryptoProviders.Add(new MockCryptographicServiceProvider());
            this._storageSubstrates = new List<IStorageSubstrate>();
            this._storageSubstrates.Add(new MockStorageSubstrate());
        }

        [TestMethod]
        public void MercurioEnvironment_Create_creates_successfully_with_valid_arguments()
        {
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, _storageSubstrates);
            var environment = MercurioEnvironment.Create(environmentScanner, PassphraseFunction);

            Assert.IsNotNull(environment);
        }

        [TestMethod]        
        [ExpectedException(typeof(ArgumentException))]
        public void MercurioEnvironment_Create_throws_with_invalid_crypto_providers()
        {
            var environmentScanner = new MockEnvironmentScanner(new List<ICryptographicServiceProvider>(), _storageSubstrates);
            var environment = MercurioEnvironment.Create(environmentScanner, PassphraseFunction);
            Assert.IsNotNull(environment);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MercurioEnvironment_Create_throws_with_invalid_storage_substrates()
        {
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, new List<IStorageSubstrate>());
            var environment = MercurioEnvironment.Create(environmentScanner, PassphraseFunction);
            Assert.IsNotNull(environment);
        }

        [TestMethod]
        [ExpectedException(typeof(MercurioException))]
        public void MercurioEnvironment_GetContainers_throws_when_identity_not_set()
        {
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, _storageSubstrates);
            var environment = MercurioEnvironment.Create(environmentScanner, PassphraseFunction);
            var containers = environment.GetContainers();
        }

        [TestMethod]
        public void MercurioEnvironment_GetContainers_returns_successful_value()
        {
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, _storageSubstrates);
            var environment = MercurioEnvironment.Create(environmentScanner, PassphraseFunction);

            var identity = environment.GetAvailableIdentities().Where(s => s.UniqueIdentifier == CryptoTestConstants.HermesPublicKeyID).FirstOrDefault();
            Assert.IsNotNull(identity);
            environment.SetActiveIdentity(identity);

            var containers = environment.GetContainers();
            Assert.IsNotNull(containers);
            Assert.IsTrue(containers.Count >= 0);
        }

        [TestMethod]
        public void MercurioEnvironment_GetAvailableStorageSubstrates_returns_successful_value()
        {
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, _storageSubstrates);
            var environment = MercurioEnvironment.Create(environmentScanner, PassphraseFunction);
            var substrates = environment.GetAvailableStorageSubstrateNames();
            Assert.IsNotNull(substrates);
            Assert.IsTrue(substrates.Count >= 0);
        }

        [TestMethod]
        [ExpectedException(typeof(MercurioException))]
        public void MercurioEnvironment_CreateContainer_throws_when_identity_not_set()
        {
            string newContainerName = "testContainer";
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, _storageSubstrates);
            var environment = MercurioEnvironment.Create(environmentScanner, PassphraseFunction);
            var substrates = environment.GetAvailableStorageSubstrateNames();
            var newContainer = environment.CreateContainer(newContainerName, substrates[0]);
        }

        [TestMethod]
        public void MercurioEnvironment_CreateContainer_creates_container()
        {
            string newContainerName = "testContainer";
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, _storageSubstrates);
            var environment = MercurioEnvironment.Create(environmentScanner, PassphraseFunction);

            var identity = environment.GetAvailableIdentities().Where(s => s.UniqueIdentifier == CryptoTestConstants.HermesPublicKeyID).FirstOrDefault();
            Assert.IsNotNull(identity);
            environment.SetActiveIdentity(identity);

            var substrates = environment.GetAvailableStorageSubstrateNames();
            var originalContainerList = environment.GetContainers();
            var newContainer = environment.CreateContainer(newContainerName, substrates[0]);
            Assert.IsNotNull(newContainer);
            var newContainerList = environment.GetContainers();
            Assert.IsTrue(newContainerList.Count == originalContainerList.Count + 1);
            Assert.IsNotNull(newContainerList.Where(s => s.Name == newContainerName).SingleOrDefault());
        }

        private NetworkCredential PassphraseFunction(string identity)
        {
            return new NetworkCredential(identity, CryptoTestConstants.HermesPassphrase);
        }
    }
}
