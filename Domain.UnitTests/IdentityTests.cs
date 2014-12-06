using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Domain.UnitTests
{
    [TestClass]
    public class IdentityTests
    {
        [TestMethod]
        public void Identity_Create_succeeds_with_correct_arguments()
        {
            var identity = Identity.Create("unique", "public");
            Assert.IsNotNull(identity);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Identity_Create_throws_when_empty_name_passed()
        {
            var identity = Identity.Create("", "public");
            Assert.IsNotNull(identity);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Identity_Create_throws_when_null_name_passed()
        {
            var identity = Identity.Create(null, "public");
            Assert.IsNotNull(identity);
        }

        [TestMethod]
        public void Identity_Equals_returns_true_when_unique_identifiers_match()
        {
            var identity1 = Identity.Create("unique-id", "publickey1");
            var identity2 = Identity.Create("unique-id", "publickey2");
            Assert.IsTrue(identity1.Equals(identity2));
        }

        [TestMethod]
        public void Identity_Equals_returns_false_when_unique_identifiers_do_not_match()
        {
            var identity1 = Identity.Create("unique-id1", "publickey");
            var identity2 = Identity.Create("unique-id2", "publickey");
            Assert.IsFalse(identity1.Equals(identity2));
            Assert.IsFalse(identity1 == identity2);
        }
    }
}
