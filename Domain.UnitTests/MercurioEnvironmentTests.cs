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

    [TestClass]
    public class ContainerTests
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
        public void MercurioEnvironment_Create_creates_successfully()
        {
            var environment = MercurioEnvironment.Create(_cryptoProviders, _storageSubstrates);
            Assert.IsNotNull(environment);
        }

        [TestMethod]
        public void MercurioEnvironment_GetContainers_returns_successful_value()
        {
            var environment = MercurioEnvironment.Create(_cryptoProviders, _storageSubstrates);
            var containers = environment.GetContainers();
            Assert.IsNotNull(containers);
            Assert.IsTrue(containers.Count >= 0);
        }

        [TestMethod]
        public void MercurioEnvironment_GetAvailableStorageSubstrates_returns_successful_value()
        {
            var environment = MercurioEnvironment.Create(_cryptoProviders, _storageSubstrates);
            var substrates = environment.GetAvailableStorageSubstrateNames();
            Assert.IsNotNull(substrates);
            Assert.IsTrue(substrates.Count >= 0);
        }

        [TestMethod]
        public void MercurioEnvironment_CreateContainer_creates_container()
        {
            string newContainerName = "testContainer";
            var environment = MercurioEnvironment.Create(_cryptoProviders, _storageSubstrates);
            var substrates = environment.GetAvailableStorageSubstrateNames();
            Assert.IsTrue(substrates.Count > 0);
            var originalContainerList = environment.GetContainers();
            var newContainer = environment.CreateContainer(newContainerName, substrates[0]);
            Assert.IsNotNull(newContainer);
            var newContainerList = environment.GetContainers();
            Assert.IsTrue(newContainerList.Count == originalContainerList.Count + 1);
            Assert.IsNotNull(newContainerList.Where(s => s.Name == newContainerName).SingleOrDefault());
        }
    }
}
