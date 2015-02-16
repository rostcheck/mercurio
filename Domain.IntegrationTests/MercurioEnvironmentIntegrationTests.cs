using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mercurio.Domain;
using Mercurio.Domain.Implementation;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Domain.IntegrationTests
{
    [TestClass]
    public class MercurioEnvironmentIntegrationTests
    {
        [TestMethod]
        public void Create_container_persists_container()
        {
            var environmentScanner = new EnvironmentScanner();
            var storageSubstrates = environmentScanner.GetStorageSubstrates();
            var environment = MercurioEnvironment.Create(environmentScanner);

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
            var environment = MercurioEnvironment.Create(environmentScanner);

            var originalContainerList = environment.GetContainers();
            var newContainerName = string.Format("TestContainer-{0}", Guid.NewGuid().ToString());
            environment.CreateContainer(newContainerName, storageSubstrates[0].Name);

            var container = environment.GetContainer(newContainerName);
            Assert.IsNotNull(container);
            container.Unlock();

            var documentName = "Thoughts About Test Documents";
            var identity = environment.GetAvailableIdentities().Where(s => s.Address == "hermes@proyectomercurio.cl").FirstOrDefault();
            Assert.IsNotNull(identity);
            var document = container.CreateTextDocument(documentName, identity, testDocumentData);
            Assert.IsNotNull(document);

            container.Lock();

            var container2 = environment.GetContainer(newContainerName);
            Assert.IsNotNull(container2);
            container2.Unlock();
            var document2 = container.Documents.Where(s => s.Name == documentName).FirstOrDefault();;
            Assert.IsNotNull(document2);
            Assert.IsTrue(document2.Content == document.Content);
            container.Lock();
        }
    }
}
