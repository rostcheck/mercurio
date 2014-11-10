using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeyRingService;

namespace TestKeyRingService
{
    [TestClass]
    public class KeyRingServiceTests
    {
        [TestMethod]
        public void GetAvailableKeyRings()
        {
            var keyRingService = new MockKeyRingService();
            var keyRings = keyRingService.GetAvailableKeyRings();
            Assert.IsTrue(keyRings.Count > 0);
        }

        [TestMethod]
        public void AddKeyRing()
        {
            var keyRingService = new MockKeyRingService();
            var priorRings = keyRingService.GetAvailableKeyRings();
            var keyRing = new KeyRing() { Name = "testKeyRing", Path = @"..\..\TestKeyRings\testKeyRing" };
            keyRingService.AddKeyRing(keyRing);
            var newKeyRings = keyRingService.GetAvailableKeyRings();
            Assert.IsTrue(newKeyRings.Count == priorRings.Count + 1);
            CollectionAssert.Contains(newKeyRings, keyRing);            
        }

        [TestMethod]
        public void CreateKeyRing()
        {
            string name = "testCreateKeyRing";
            var keyRingService = new MockKeyRingService();
            var priorRings = keyRingService.GetAvailableKeyRings();
            var newKeyRing = keyRingService.CreateKeyRing(name);
            var newKeyRings = keyRingService.GetAvailableKeyRings();
            Assert.IsTrue(newKeyRings.Count == priorRings.Count + 1);
            CollectionAssert.Contains(newKeyRings, newKeyRing);
            Assert.IsTrue(newKeyRing.Name == name);
            Assert.IsTrue(newKeyRing.Path != string.Empty);
        }
    }
}
