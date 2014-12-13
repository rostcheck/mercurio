using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Mercurio.Domain.UnitTests
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void User_Create_throws_when_null_identity_passed()
        {
            var user = User.Create("user1", (Identity)null);
            var identities = user.GetIdentities();
            Assert.IsTrue(identities.Count == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void User_Create_throws_when_empty_name_passed()
        {
            var user = User.Create("", Identity.Create("unique", "public"));
            var identities = user.GetIdentities();
            Assert.IsTrue(identities.Count == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void User_Create_throws_when_null_name_passed()
        {
            var user = User.Create(null, Identity.Create("unique", "public"));
            var identities = user.GetIdentities();
            Assert.IsTrue(identities.Count == 0);
        }

        [TestMethod]
        public void User_GetIdentities_returns_identities_when_some_exist()
        {
            var originalIdentities = new List<Identity>();
            originalIdentities.Add(Identity.Create("unique-id", "public-key"));
            var user = User.Create("user1", originalIdentities);
            var identities = user.GetIdentities();
            Assert.IsTrue(identities.Count == originalIdentities.Count);
            CollectionAssert.AreEquivalent(originalIdentities, identities);
        }
    }
}
