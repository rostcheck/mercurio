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
            var user = MercurioUser.Create("user1", (Identity)null);
            var identities = user.GetIdentities();
            Assert.IsTrue(identities.Count == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void User_Create_throws_when_empty_name_passed()
        {
            var identity = Identity.Create("unique-id", "Alice Smith", "alice@mercurio.org", "Alice's Personal Account");
            var user = MercurioUser.Create("", identity);
            var identities = user.GetIdentities();
            Assert.IsTrue(identities.Count == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void User_Create_throws_when_null_name_passed()
        {
            var identity = Identity.Create("unique-id", "Alice Smith", "alice@mercurio.org", "Alice's Personal Account");
            var user = MercurioUser.Create(null, identity);
            var identities = user.GetIdentities();
            Assert.IsTrue(identities.Count == 0);
        }

        [TestMethod]
        public void User_GetIdentities_returns_identities_when_some_exist()
        {
            var originalIdentities = new List<Identity>();
            var identity = Identity.Create("unique-id", "Alice Smith", "alice@mercurio.org", "Alice's Personal Account");
            originalIdentities.Add(identity);
            var user = MercurioUser.Create("user1", originalIdentities);
            var identities = user.GetIdentities();
            Assert.IsTrue(identities.Count == originalIdentities.Count);
            CollectionAssert.AreEquivalent(originalIdentities, identities);
        }
    }
}
