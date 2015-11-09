using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mercurio.Domain.Implementation;
using Mercurio.Domain;
using Entities;
using Cryptography.GPG;
using System.Net;
using TestUtilities;

namespace Domain.IntegrationTests
{
    [TestClass]
    public class DiskStorageSubstrateTests
    {
        [TestMethod]
        public void DiskStorageSubstrate_Create_constructs_with_valid_path()
        {
            var storageSubstrate = DiskStorageSubstrate.Create(".", SerializerType.BinarySerializer);
            Assert.IsNotNull(storageSubstrate);
            Assert.IsNotNull(storageSubstrate.Name);
            Assert.IsFalse(storageSubstrate.Name == "");
        }

        [TestMethod]
        [ExpectedException(typeof(System.IO.DirectoryNotFoundException))]
        public void DiskStorageSubstrate_Create_throws_with_invalid_path()
        {
            var storageSubstrate = DiskStorageSubstrate.Create("c:\\invalid-path", SerializerType.BinarySerializer);
        }

        public void DiskStorageSubstrate_CreateContainer_creates_container()
        {
            string testContainerName = "testContainer";
            var configuration = TestUtilities.TestConfig.Create("Bob");
            CryptoManagerFactory.Register(CryptoType.GPG.ToString(), typeof(CrypographicServiceProviderGPG));
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();

            var storageSubstrate = DiskStorageSubstrate.Create(".", SerializerType.BinarySerializer);
            var cryptoManager = CryptoManagerFactory.Create(CryptoType.GPG.ToString(), osAbstractor, configuration);
            var credential = new NetworkCredential(CryptoTestConstants.HermesPublicKeyID, CryptoTestConstants.HermesPassphrase);
            cryptoManager.SetCredential(credential);
            var container = storageSubstrate.CreateContainer(testContainerName, cryptoManager);
            var containers = storageSubstrate.GetAllContainers();
            Assert.IsTrue(containers.Count == 1);
            Assert.IsTrue(containers[0].Name == testContainerName);
            Assert.IsTrue(storageSubstrate.HostsContainer(containers[0].Id) == true);  
        }

        public void DiskStorageSubstrate_DeleteContainer_deletes_container() 
        {
            string testContainerName = "testContainer";
            var configuration = TestUtilities.TestConfig.Create("Bob");
            CryptoManagerFactory.Register(CryptoType.GPG.ToString(), typeof(CrypographicServiceProviderGPG));

            var storageSubstrate = DiskStorageSubstrate.Create(".", SerializerType.BinarySerializer);
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            var cryptoManager = CryptoManagerFactory.Create(CryptoType.GPG.ToString(), osAbstractor, configuration);
            var container = storageSubstrate.CreateContainer(testContainerName, cryptoManager);
            var containers = new List<IContainer>(storageSubstrate.GetAllContainers());
            Assert.IsTrue(containers.Count == 1);
            storageSubstrate.DeleteContainer(containers[0].Id);
            var savedId = containers[0].Id;
            Assert.IsTrue(storageSubstrate.GetAllContainers().Count == 0);
            Assert.IsFalse(storageSubstrate.HostsContainer(savedId));
        }

        public void DiskStorageSubstrate_RetrievePrivateMetadataBytes_returns_encrypted_data()
        {
            string testContainerName = "testContainer";
            var configuration = TestUtilities.TestConfig.Create("Bob");
            CryptoManagerFactory.Register(CryptoType.GPG.ToString(), typeof(CrypographicServiceProviderGPG));

            var storageSubstrate = DiskStorageSubstrate.Create(".", SerializerType.BinarySerializer);
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            var cryptoManager = CryptoManagerFactory.Create(CryptoType.GPG.ToString(), osAbstractor, configuration);
            var container = storageSubstrate.CreateContainer(testContainerName, cryptoManager);
            var bytes = storageSubstrate.RetrievePrivateMetadataBytes(container.Id);
            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length > 0);
        }

        // TODO: build out tests for retrieving content
        /*
        public void DiskStorageSubstrate_StoreDocumentVersion_returns_encrypted_data()
        {            
            string testContainerName = "testContainer";
            var configuration = TestUtilities.TestConfig.Create("Bob");
            CryptoManagerFactory.Register(CryptoType.GPG.ToString(), typeof(CrypographicServiceProviderGPG));

            var storageSubstrate = DiskStorageSubstrate.Create(".", SerializerType.BinarySerializer);
            var cryptoManager = CryptoManagerFactory.Create(CryptoType.GPG.ToString(), configuration);
            //cryptoManager.
            var container = storageSubstrate.CreateContainer(testContainerName, cryptoManager);

            string testDocumentContent = "This is the content of the test document";
            var documentId = Guid.NewGuid();
            var documentVersion = container.CreateTextDocument("testDocument", cryptoManager.(), testDocumentContent);
            storageSubstrate.StoreDocumentVersion(container.Id, documentVersion);

            var retrievedDocumentVersion = storageSubstrate.RetrieveDocumentVersion(container.Id, documentVersion.Metadata);
            Assert.IsNotNull(retrievedDocumentVersion);
            Assert.IsTrue(documentVersion.Id == retrievedDocumentVersion.Id);
            Assert.IsTrue(documentVersion.CreatorId == retrievedDocumentVersion.CreatorId);
            Assert.IsTrue(documentVersion.CreatedDateTime == retrievedDocumentVersion.CreatedDateTime);
            Assert.IsTrue(documentVersion.DocumentContent == retrievedDocumentVersion.DocumentContent);
            Assert.IsTrue(retrievedDocumentVersion.DocumentContent == testDocumentContent);
            Assert.IsTrue(documentVersion.DocumentId == retrievedDocumentVersion.DocumentId);
            Assert.IsTrue(documentVersion.PriorVersionId == retrievedDocumentVersion.PriorVersionId);

        }
        void StoreDocumentVersion(Guid containerId, DocumentVersion documentVersion);
        DocumentVersion RetrieveDocumentVersion(Guid containerId, DocumentVersionMetadata documentVersionMetadata);
        void StoreMetadata(Guid containerId, ContainerMetadata metadata);
        void StorePrivateMetadata(Guid containerId, Stream encryptedPrivateMetadata);
         * */

    }
}
