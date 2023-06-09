﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mercurio.Domain;
using Mercurio.Domain.Implementation;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TestCryptography;
using TestUtilities;

namespace Domain.IntegrationTests
{
    [TestClass]
    public class MercurioEnvironmentIntegrationTests
    {
        public const string TestUserName = "mercurio";

        [TestInitialize]
        public void TestInitialize()
        {
            TestUtils.CleanupSubstrate(ConfigurationManager.GetConfigurationValue("StorageSubstrate"));
            TestUtils.SetupUserDir(TestUserName);
            TestUtils.SwitchUser(null, TestUserName);
        }

        [TestMethod]
        public void Create_container_persists_container()
        {
            var environmentScanner = new EnvironmentScanner(TestUtils.GetUserWorkingDir(TestUserName));
            var storageSubstrates = environmentScanner.GetStorageSubstrates();
            var serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            var environment = MercurioEnvironment.Create(environmentScanner, osAbstractor, serializer, TestUtils.PassphraseFunction);
            environment.SetUserHomeDirectory(TestUtils.GetUserWorkingDir(TestUserName));
            var identity = environment.GetAvailableIdentities().Where(s => s.UniqueIdentifier == CryptoTestConstants.HermesPublicKeyID).FirstOrDefault();
            environment.SetActiveIdentity(identity);

            var originalContainerList = environment.GetContainers();
            var newContainerName = string.Format("TestContainer-{0}", Guid.NewGuid().ToString());
            environment.CreateContainer(newContainerName, storageSubstrates[0].Name);

            var containers = environment.GetContainers();
            Assert.IsTrue(environment.GetContainers().Where(s => s.Name == newContainerName).FirstOrDefault() != null);
        }

        [TestMethod]
        public void CreateTextDocument_creates_document()
        {
            const string testDocumentData = @"These are the contents of the test document. One, two, three. Here they are. If you have any questions, you can contact me via telepathy, or Mercurio message.";
            var environmentScanner = new EnvironmentScanner();
            var storageSubstrates = environmentScanner.GetStorageSubstrates();
            var serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            var environment = MercurioEnvironment.Create(environmentScanner, osAbstractor, serializer, TestUtils.PassphraseFunction);
            environment.SetUserHomeDirectory(TestUtils.GetUserWorkingDir(TestUserName));

            var identity = environment.GetAvailableIdentities().Where(s => s.UniqueIdentifier == CryptoTestConstants.HermesPublicKeyID).FirstOrDefault();
            environment.SetActiveIdentity(identity);

            var originalContainerList = environment.GetContainers();
            var newContainerName = string.Format("TestContainer-{0}", Guid.NewGuid().ToString());
            var c1 = environment.CreateContainer(newContainerName, storageSubstrates[0].Name);

            var container = environment.GetContainer(newContainerName);
            Assert.IsNotNull(container);
            environment.UnlockContainer(container);

            var documentName = "Thoughts About Test Documents";
            Assert.IsNotNull(identity);
            var documentVersion = container.CreateTextDocument(documentName, identity, testDocumentData);
            Assert.IsNotNull(documentVersion);

            environment.LockContainer(container);

            var container2 = environment.GetContainer(newContainerName);
            Assert.IsNotNull(container2);
            environment.UnlockContainer(container2);
            var documentVersionAgain = container2.Documents.Where(s => s == documentName).FirstOrDefault();;
            Assert.IsNotNull(documentVersionAgain);
            container2.GetLatestDocumentVersion(documentName);
            var documentVersion2 = container2.GetLatestDocumentVersion(documentName);
            Assert.IsTrue(documentVersion2.DocumentContent == documentVersion.DocumentContent);
            environment.LockContainer(container);
        }
    }
}
