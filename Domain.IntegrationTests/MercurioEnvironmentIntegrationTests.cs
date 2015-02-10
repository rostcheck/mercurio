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
        public void MercurioEnvironment_Create_container_persists_container()
        {
            var environmentScanner = new EnvironmentScanner();
            var storageSubstrates = environmentScanner.GetStorageSubstrates();
            var storagePlans = environmentScanner.GetStoragePlans();
            var environment = MercurioEnvironment.Create(environmentScanner);

            var originalContainerList = environment.GetContainers();
            var newContainerName = string.Format("TestContainer-{0}", Guid.NewGuid().ToString());
            environment.CreateContainer(newContainerName, storageSubstrates[0].Name, storagePlans[0].Name);

            Assert.IsTrue(environment.GetContainers().Where(s => s.Name == newContainerName).FirstOrDefault() != null);
        }


    }
}
