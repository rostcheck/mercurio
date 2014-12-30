using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mercurio.Domain.Implementation;

namespace Domain.IntegrationTests
{
    [TestClass]
    public class DiskStorageSubstrateTests
    {
        [TestMethod]
        public void DiskStorageSubstrate_constructs_with_valid_path()
        {
            var storageSubstrate = DiskStorageSubstrate.Create(Environment.SpecialFolder.MyDocuments.ToString());
            Assert.IsNotNull(storageSubstrate);
            Assert.IsNotNull(storageSubstrate.Name);
            Assert.IsFalse(storageSubstrate.Name == "");
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void DiskStorageSubstrate_throws_with_invalid_path()
        {
            var storageSubstrate = DiskStorageSubstrate.Create("c:\\invalid-path");
        }
    }
}
