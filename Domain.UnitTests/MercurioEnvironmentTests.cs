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
    }

    internal class MockStorageSubstrate : IStorageSubstrate
    {
        private List<Container> _containers;

        public MockStorageSubstrate()
        {
            _containers = new List<Container>();
        }

        public string Name
        {
            get { return "MockStorageSubstate"; }
        }

        public IEnumerable<Container> GetContainers()
        {
            return new List<Container>(_containers);
        }


        public void AddContainer(Container container)
        {
            if (container == null)
                throw new ArgumentNullException();

            _containers.Add(container);
        }
    }

    internal class MockStoragePlan : IStoragePlan
    {
        public string Name
        {
            get { return "MockStoragePlan"; }
        }
    }

    internal class MockEnvironmentScanner : IEnvironmentScanner
    {
        private List<ICryptographicServiceProvider> _cryptoProviderList;
        private List<IStorageSubstrate> _storageSubstrateList;
        private List<IStoragePlan> _storagePlanList;

        public MockEnvironmentScanner(List<ICryptographicServiceProvider> cryptoProviderList,  
            List<IStorageSubstrate> storageSubstrateList,
            List<IStoragePlan> storagePlanList)
        {
            _cryptoProviderList = cryptoProviderList;
            _storageSubstrateList = storageSubstrateList;
            _storagePlanList = storagePlanList;
        }

        public List<ICryptographicServiceProvider> GetCryptographicProviders()
        {
            return _cryptoProviderList;
        }

        public List<IStorageSubstrate> GetStorageSubstrates()
        {
            return _storageSubstrateList;
        }

        public List<IStoragePlan> GetStoragePlans()
        {
            return _storagePlanList;
        }
    }

    [TestClass]
    public class MercurioEnvironmentTests
    {
        private List<ICryptographicServiceProvider> _cryptoProviders;
        private List<IStorageSubstrate> _storageSubstrates;
        private List<IStoragePlan> _storagePlans;

        [TestInitialize]
        public void ContainerTests_Initialize()
        {
            this._cryptoProviders = new List<ICryptographicServiceProvider>();
            this._cryptoProviders.Add(new MockCryptographicServiceProvider());
            this._storageSubstrates = new List<IStorageSubstrate>();
            this._storageSubstrates.Add(new MockStorageSubstrate());
            this._storagePlans = new List<IStoragePlan>();
            this._storagePlans.Add(new MockStoragePlan());
        }

        [TestMethod]
        public void MercurioEnvironment_Create_creates_successfully_with_valid_arguments()
        {
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, _storageSubstrates, _storagePlans);
            var environment = MercurioEnvironment.Create(environmentScanner);
            Assert.IsNotNull(environment);
        }

        [TestMethod]        
        [ExpectedException(typeof(ArgumentException))]
        public void MercurioEnvironment_Create_throws_with_invalid_crypto_providers()
        {
            var environmentScanner = new MockEnvironmentScanner(new List<ICryptographicServiceProvider>(), _storageSubstrates, _storagePlans);
            var environment = MercurioEnvironment.Create(environmentScanner);
            Assert.IsNotNull(environment);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MercurioEnvironment_Create_throws_with_invalid_storage_substrates()
        {
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, new List<IStorageSubstrate>(), _storagePlans);
            var environment = MercurioEnvironment.Create(environmentScanner);
            Assert.IsNotNull(environment);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MercurioEnvironment_Create_throws_with_invalid_storage_plans()
        {
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, _storageSubstrates, new List<IStoragePlan>());
            var environment = MercurioEnvironment.Create(environmentScanner);
            Assert.IsNotNull(environment);
        }

        [TestMethod]
        public void MercurioEnvironment_GetContainers_returns_successful_value()
        {
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, _storageSubstrates, _storagePlans);
            var environment = MercurioEnvironment.Create(environmentScanner);
            var containers = environment.GetContainers();
            Assert.IsNotNull(containers);
            Assert.IsTrue(containers.Count >= 0);
        }

        [TestMethod]
        public void MercurioEnvironment_GetAvailableStorageSubstrates_returns_successful_value()
        {
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, _storageSubstrates, _storagePlans);
            var environment = MercurioEnvironment.Create(environmentScanner);
            var substrates = environment.GetAvailableStorageSubstrateNames();
            Assert.IsNotNull(substrates);
            Assert.IsTrue(substrates.Count >= 0);
        }

        [TestMethod]
        public void MercurioEnvironment_CreateContainer_creates_container()
        {
            string newContainerName = "testContainer";
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, _storageSubstrates, _storagePlans);
            var environment = MercurioEnvironment.Create(environmentScanner);
            var substrates = environment.GetAvailableStorageSubstrateNames();
            var storagePlans = environment.GetAvailableStoragePlanNames();
            var originalContainerList = environment.GetContainers();
            var newContainer = environment.CreateContainer(newContainerName, substrates[0], storagePlans[0]);
            Assert.IsNotNull(newContainer);
            var newContainerList = environment.GetContainers();
            Assert.IsTrue(newContainerList.Count == originalContainerList.Count + 1);
            Assert.IsNotNull(newContainerList.Where(s => s.Name == newContainerName).SingleOrDefault());
        }
    }
}
