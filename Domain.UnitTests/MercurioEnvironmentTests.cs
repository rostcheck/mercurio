using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mercurio.Domain;
using System.Collections.Generic;
using System.Net;
using Mercurio.Domain.Implementation;
using System.IO;
using TestUtilities;
using Mercurio.Domain.TestMocks;

namespace Mercurio.Domain.UnitTests
{
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
            var serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            var environment = MercurioEnvironment.Create(environmentScanner, osAbstractor, serializer, PassphraseFunction);

            Assert.IsNotNull(environment);
        }

        [TestMethod]        
        [ExpectedException(typeof(ArgumentException))]
        public void MercurioEnvironment_Create_throws_with_invalid_crypto_providers()
        {
            var environmentScanner = new MockEnvironmentScanner(new List<ICryptographicServiceProvider>(), _storageSubstrates);
            var serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            var environment = MercurioEnvironment.Create(environmentScanner, osAbstractor, serializer, PassphraseFunction);
            Assert.IsNotNull(environment);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MercurioEnvironment_Create_throws_with_invalid_storage_substrates()
        {
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, new List<IStorageSubstrate>());
            var serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            var environment = MercurioEnvironment.Create(environmentScanner, osAbstractor, serializer, PassphraseFunction);
            Assert.IsNotNull(environment);
        }

        [TestMethod]
        [ExpectedException(typeof(MercurioException))]
        public void MercurioEnvironment_GetContainers_throws_when_identity_not_set()
        {
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, _storageSubstrates);
            var serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            var environment = MercurioEnvironment.Create(environmentScanner, osAbstractor, serializer, PassphraseFunction);
            var containers = environment.GetContainers();
        }

        [TestMethod]
        public void MercurioEnvironment_GetContainers_returns_successful_value()
        {
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, _storageSubstrates);
            var serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            var environment = MercurioEnvironment.Create(environmentScanner, osAbstractor, serializer, PassphraseFunction);

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
            var serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            var environment = MercurioEnvironment.Create(environmentScanner, osAbstractor, serializer, PassphraseFunction);
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
            var serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            var environment = MercurioEnvironment.Create(environmentScanner, osAbstractor, serializer, PassphraseFunction);
            var substrates = environment.GetAvailableStorageSubstrateNames();
            var newContainer = environment.CreateContainer(newContainerName, substrates[0]);
        }

        [TestMethod]
        public void MercurioEnvironment_CreateContainer_creates_container()
        {
            string newContainerName = "testContainer";
            var environmentScanner = new MockEnvironmentScanner(_cryptoProviders, _storageSubstrates);
            var serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            var environment = MercurioEnvironment.Create(environmentScanner, osAbstractor, serializer, PassphraseFunction);

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
