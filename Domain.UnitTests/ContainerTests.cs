using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace Mercurio.Domain.UnitTests
{
    [TestClass]
    public class ContainerTests
    {
        private Identity _identity;
        private MercurioUser _user;
        private const string _cryptoManagerType = "GPG";
        private ICryptoManager _cryptoManager;
        private IStorageSubstrate _storageSubstrate = new InMemoryStorageSubstrate();


        [TestInitialize]
        public void ContainerTests_Initialize()
        {
            _identity = Identity.Create("alice", "Alice Smith", "alice@mercurio.org", "Alice's Personal Account", _cryptoManagerType);
            _user = MercurioUser.Create("Test User", _identity);
            _cryptoManager = new MockCryptoManager();
            _cryptoManager.SetCredential(new NetworkCredential(_identity.UniqueIdentifier, "Alice's fake password"));
        }

        [TestMethod]
        public void Container_Create_creates_unlocked_container()
        {
            var container = _storageSubstrate.CreateContainer("Container that keeps all revisions", _cryptoManager, RevisionRetentionPolicyType.KeepAll);
            Assert.IsFalse(container.IsLocked);
        }

        [TestMethod]
        public void Container_CreateTextDocument_creates_document_correctly()
        {
            var container = _storageSubstrate.CreateContainer("Container that keeps all revisions", _cryptoManager, RevisionRetentionPolicyType.KeepAll);
            string documentName = "Test document";
            string initialValue = "this is my initial value";
            var textDocumentVersion = container.CreateTextDocument(documentName, _identity, initialValue);
            Assert.IsTrue(textDocumentVersion != null);
            Assert.IsTrue(textDocumentVersion.DocumentId != Guid.Empty);
            Assert.IsTrue(textDocumentVersion.Id != Guid.Empty);
            Assert.IsTrue(textDocumentVersion.DocumentContent == initialValue);
            Assert.IsTrue(container.ListAvailableVersions(documentName).Count == 1);
        }

        [TestMethod]
        public void Container_SetContent_modifies_document_content_correctly()
        {
            var container = _storageSubstrate.CreateContainer("Container that keeps all revisions", _cryptoManager, RevisionRetentionPolicyType.KeepAll);
            string documentName = "Test document";
            string initialValue = "this is my initial value";
            var textDocumentVersion = container.CreateTextDocument(documentName, _identity, initialValue);            
            string newValue = initialValue + " and this is a revised value";
            container.ModifyTextDocument(documentName, _identity, newValue);
            var newVersion = container.GetLatestDocumentVersion(documentName);
            Assert.IsFalse(textDocumentVersion.DocumentContent == newValue);
            Assert.IsTrue(newVersion.DocumentContent == newValue);
        }

        [TestMethod]
        public void Container_retains_all_revisions_when_retention_policy_is_keep_all()
        {
            var container = _storageSubstrate.CreateContainer("Container that keeps all revisions", _cryptoManager, RevisionRetentionPolicyType.KeepAll);
            string documentName = "Test document";
            string initialValue = "this is my initial value";
            var textDocumentVersion = container.CreateTextDocument(documentName, _identity, initialValue);
            string newValue = initialValue + " and this is a revised value";
            container.ModifyTextDocument(documentName, _identity, newValue);

            Assert.IsTrue(container.ListAvailableVersions("Test document").Count == 2);
        }

        [TestMethod]
        public void Container_retains_one_revision_when_retention_policy_is_keep_one()
        {
            var container = _storageSubstrate.CreateContainer("Container that keeps one revision", _cryptoManager, RevisionRetentionPolicyType.KeepOne);
            string documentName = "Test document";
            string initialValue = "this is my initial value";
            var textDocumentVersion = container.CreateTextDocument(documentName, _identity, initialValue);
            string newValue = initialValue + " and this is a revised value";
            container.ModifyTextDocument(documentName, _identity, newValue);
            var newVersion = container.GetLatestDocumentVersion(documentName);

            Assert.IsTrue(container.ListAvailableVersions("Test document").Count == 1);
            Assert.IsTrue(newVersion.DocumentContent == newValue);
        }

        [TestMethod]
        public void Container_retains_one_revision_when_using_default_retention_policy()
        {
            var container = _storageSubstrate.CreateContainer("Container that keeps one revision", _cryptoManager);
            string documentName = "Test document";
            string initialValue = "this is my initial value";
            var textDocument = container.CreateTextDocument("Test document", _identity, initialValue);
            string newValue = initialValue + " and this is a revised value";
            var newVersion = container.ModifyTextDocument(documentName, _identity, newValue);

            Assert.IsFalse(textDocument.DocumentContent == newValue);
            Assert.IsTrue(newVersion.DocumentContent == newValue);
            Assert.IsTrue(container.ListAvailableVersions("Test document").Count == 1);
        }

        [TestMethod]
        public void Container_shows_correct_number_of_documents()
        {
            var container = _storageSubstrate.CreateContainer("Container that keeps one revision", _cryptoManager);
            string initialValue = "initial value for document 1";
            var textDocument1 = container.CreateTextDocument("Test document 1", _identity, initialValue);
            initialValue = "initial value for document 2";
            var textDocument2 = container.CreateTextDocument("Test document 2", _identity, initialValue);
            
            Assert.IsTrue(container.Documents.Count == 2);
            Assert.IsTrue(container.Documents.Where(s => s == "Test document 1").SingleOrDefault() != null);
            Assert.IsTrue(container.Documents.Where(s => s == "Test document 2").SingleOrDefault() != null);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Container_document_access_throws_if_not_unlocked()
        {
            var container = _storageSubstrate.CreateContainer("Container that keeps one revision", _cryptoManager);
            string initialValue = "initial value for document 1";
            var textDocument1 = container.CreateTextDocument("Test document 1", _identity, initialValue);
            container.Lock();
            Assert.IsTrue(container.Documents.Count == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Container_CreateTextDocument_throws_if_not_unlocked()
        {
            var container = _storageSubstrate.CreateContainer("Container that keeps one revision", _cryptoManager);
            container.Lock();
            string initialValue = "initial value for document 1";
            var textDocument1 = container.CreateTextDocument("Test document 1", _identity, initialValue);
            Assert.IsTrue(container.Documents.Count == 1);
        }
    }
}
