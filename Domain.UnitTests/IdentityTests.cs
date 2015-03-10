using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mercurio.Domain.UnitTests
{
    [TestClass]
    public class IdentityTests
    {
        private const string _cryptoManagerType = "GPG";

        [TestMethod]
        public void Identity_Create_succeeds_with_correct_arguments()
        {
            var identity = Identity.Create("alice", "Alice Smith", "alice@mercurio.org", "Alice's Personal Account", _cryptoManagerType);
            Assert.IsNotNull(identity);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Identity_Create_throws_when_empty_identifier_passed()
        {
            var identity = Identity.Create("", "Alice Smith", "alice@mercurio.org", "Alice's Personal Account", _cryptoManagerType);
            Assert.IsNotNull(identity);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Identity_Create_throws_when_null_identifier_passed()
        {
            var identity = Identity.Create(null, "Alice Smith", "alice@mercurio.org", "Alice's Personal Account", _cryptoManagerType);
            Assert.IsNotNull(identity);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Identity_Create_throws_when_empty_name_passed()
        {
            var identity = Identity.Create("alice", "", "alice@mercurio.org", "Alice's Personal Account", _cryptoManagerType);
            Assert.IsNotNull(identity);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Identity_Create_throws_when_null_name_passed()
        {
            var identity = Identity.Create("alice", null, "alice@mercurio.org", "Alice's Personal Account", _cryptoManagerType);
            Assert.IsNotNull(identity);
        }

        [TestMethod]
        public void Identity_Equals_returns_true_when_unique_identifiers_match()
        {
            var identity1 = Identity.Create("unique-id", "Alice Smith", "alice@mercurio.org", "Alice's Personal Account", _cryptoManagerType);
            var identity2 = Identity.Create("unique-id", "Alice Smith", "alice@mercurio.org", "Alice's Personal Account", _cryptoManagerType);
            Assert.IsTrue(identity1.Equals(identity2));
        }

        [TestMethod]
        public void Identity_Equals_returns_false_when_unique_identifiers_do_not_match()
        {
            var identity1 = Identity.Create("unique-id1", "Alice Smith", "alice@mercurio.org", "Alice's Personal Account", _cryptoManagerType);
            var identity2 = Identity.Create("unique-id2", "Alice Smith", "alice@mercurio.org", "Alice's Personal Account", _cryptoManagerType);
            Assert.IsFalse(identity1.Equals(identity2));
            Assert.IsFalse(identity1 == identity2);
        }
    }
}
