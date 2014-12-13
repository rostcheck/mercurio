using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mercurio.Domain.IntegrationTests
{
    [TestClass]
    public class ContainerTests
    {
        [TestMethod]
        public void Container_retains_all_revisions_when_retention_policy_is_keep_all()
        {
            var identity = Identity.Create("davidr-1", "public-key", "private-key");
            var user = User.Create("Test user", identity);

            var container = Container.Create("Container that keeps all revisions", RevisionRetentionPolicyType.KeepAll);
            string initialValue = "this is my initial value";
            var textDocument = container.CreateTextDocument("Test document", identity, initialValue);
            string newValue = initialValue + " and this is a revised value";
            textDocument.SetContent(newValue, identity);

            Assert.IsTrue(textDocument.Content == newValue);
            Assert.IsTrue(textDocument.Revisions.Count == 2);
        }

        [TestMethod]
        public void Container_retains_one_revision_when_retention_policy_is_keep_one()
        {
            var identity = Identity.Create("davidr-1", "public-key", "private-key");
            var user = User.Create("Test user", identity);

            var container = Container.Create("Container that keeps one revision", RevisionRetentionPolicyType.KeepOne);
            string initialValue = "this is my initial value";
            var textDocument = container.CreateTextDocument("Test document", identity, initialValue);
            string newValue = initialValue + " and this is a revised value";
            textDocument.SetContent(newValue, identity);

            Assert.IsTrue(textDocument.Content == newValue);
            Assert.IsTrue(textDocument.Revisions.Count == 1);
        }

        [TestMethod]
        public void Container_retains_one_revision_when_using_default_retention_policy()
        {
            var identity = Identity.Create("davidr-1", "public-key", "private-key");
            var user = User.Create("Test user", identity);

            var container = Container.Create("Container that keeps one revision");
            string initialValue = "this is my initial value";
            var textDocument = container.CreateTextDocument("Test document", identity, initialValue);
            string newValue = initialValue + " and this is a revised value";
            textDocument.SetContent(newValue, identity);

            Assert.IsTrue(textDocument.Content == newValue);
            Assert.IsTrue(textDocument.Revisions.Count == 1);
        }
            //document.ChangeElement(AtomicDataElementChange.SetValue("name1", "value1", DataElementType.String));
            //document.CommitChanges(identity.UniqueIdentifier);
            //document.ChangeElement(AtomicDataElementChange.SetValue("name1", "value1", DataElementType.String));
            //document.CommitChanges(identity.UniqueIdentifier);
            //document.GetRevisions();
    }
}
