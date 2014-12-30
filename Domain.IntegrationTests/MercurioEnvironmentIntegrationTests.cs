using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mercurio.Domain;
using Mercurio.Domain.Implementation;

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
            environment.CreateContainer(string.Format("TestContainer-{0}", Guid.NewGuid().ToString()), storageSubstrates[0].Name, storagePlans[0].Name);

        }
    }
}
