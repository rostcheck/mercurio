using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mercurio.Domain;
using System.Collections.Generic;

namespace Mercurio.Domain.UnitTests
{
    internal class MockCryptographicServiceProvider : ICryptographicServiceProvider
    {

        public string GetProviderType()
        {
            return "MockCryptoProvider";
        }

        public bool IsInstalled()
        {
            return true;
        }


        public CryptoManagerConfiguration GetConfiguration()
        {
            throw new NotImplementedException();
        }

        public ICryptoManager CreateManager(CryptoManagerConfiguration configuration)
        {
            throw new NotImplementedException();
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

        public IEnumerable<IContainer> GetContainers(List<ICryptographicServiceProvider> availableProviders)
        {
            return new List<IContainer>(_containers);
        }

        public IContainer CreateContainer(string containerName, ICryptographicServiceProvider cryptoProvider, RevisionRetentionPolicyType retentionPolicy)
        {
            var container = Container.Create(containerName, retentionPolicy);
            _containers.Add(container);
            return container;
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
            var environment = MercurioEnvironment.Create(environmentScanner);
            Assert.IsNotNull(environment);
        }

        [TestMethod]        
        [ExpectedException(typeof(ArgumentException))]
        public void MercurioEnvironment_Create_throws_with_invalid_crypto_providers()
        {
            var environmentScanner = new MockEnvironmentScanner(new List<ICryptographicServiceProvider>(), _storageSubstrates);
            var environment = MercurioEnvironment.Create(environmentScanner);
            Assert.IsNotNull(environment);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MercurioEnvironment_Create_throws_with_invalid_storage_substrates()
        {
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, new List<IStorageSubstrate>());
            var environment = MercurioEnvironment.Create(environmentScanner);
            Assert.IsNotNull(environment);
        }

        [TestMethod]
        public void MercurioEnvironment_GetContainers_returns_successful_value()
        {
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, _storageSubstrates);
            var environment = MercurioEnvironment.Create(environmentScanner);
            var containers = environment.GetContainers();
            Assert.IsNotNull(containers);
            Assert.IsTrue(containers.Count >= 0);
        }

        [TestMethod]
        public void MercurioEnvironment_GetAvailableStorageSubstrates_returns_successful_value()
        {
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, _storageSubstrates);
            var environment = MercurioEnvironment.Create(environmentScanner);
            var substrates = environment.GetAvailableStorageSubstrateNames();
            Assert.IsNotNull(substrates);
            Assert.IsTrue(substrates.Count >= 0);
        }

        [TestMethod]
        public void MercurioEnvironment_CreateContainer_creates_container()
        {
            string newContainerName = "testContainer";
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, _storageSubstrates);
            var environment = MercurioEnvironment.Create(environmentScanner);
            var substrates = environment.GetAvailableStorageSubstrateNames();
            var originalContainerList = environment.GetContainers();
            var newContainer = environment.CreateContainer(newContainerName, substrates[0]);
            Assert.IsNotNull(newContainer);
            var newContainerList = environment.GetContainers();
            Assert.IsTrue(newContainerList.Count == originalContainerList.Count + 1);
            Assert.IsNotNull(newContainerList.Where(s => s.Name == newContainerName).SingleOrDefault());
        }
    }
}
