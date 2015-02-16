﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mercurio.Domain.UnitTests
{
    [TestClass]
    public class ContainerTests
    {
        private Identity _identity;
        private MercurioUser _user;

        [TestInitialize]
        public void ContainerTests_Initialize()
        {
            _identity = Identity.Create("alice", "Alice Smith", "alice@mercurio.org", "Alice's Personal Account");
            _user = MercurioUser.Create("Test User", _identity);
        }

        [TestMethod]
        public void Container_Create_creates_unlocked_container()
        {
            var container = Container.Create("Container that keeps all revisions", RevisionRetentionPolicyType.KeepAll);
            Assert.IsFalse(container.IsLocked);
        }

        [TestMethod]
        public void Container_CreateTextDocument_creates_document_correctly()
        {
            var container = Container.Create("Container that keeps all revisions", RevisionRetentionPolicyType.KeepAll);
            string initialValue = "this is my initial value";
            string documentName = "Test document";
            var textDocument = container.CreateTextDocument(documentName, _identity, initialValue);
            Assert.IsTrue(textDocument.Name == documentName);
            Assert.IsTrue(textDocument.Content == initialValue);
        }

        [TestMethod]
        public void Container_SetContent_modifies_document_content_correctly()
        {
            var container = Container.Create("Container that keeps all revisions", RevisionRetentionPolicyType.KeepAll);
            string initialValue = "this is my initial value";
            var textDocument = container.CreateTextDocument("Test document", _identity, initialValue);            
            string newValue = initialValue + " and this is a revised value";
            textDocument.SetContent(newValue, _identity);
            Assert.IsTrue(textDocument.Content == newValue);
        }

        [TestMethod]
        public void Container_retains_all_revisions_when_retention_policy_is_keep_all()
        {
            var container = Container.Create("Container that keeps all revisions", RevisionRetentionPolicyType.KeepAll);
            string initialValue = "this is my initial value";
            var textDocument = container.CreateTextDocument("Test document", _identity, initialValue);
            string newValue = initialValue + " and this is a revised value";
            textDocument.SetContent(newValue, _identity);

            Assert.IsTrue(textDocument.Content == newValue);
            Assert.IsTrue(textDocument.Revisions.Count == 2);
        }

        [TestMethod]
        public void Container_retains_one_revision_when_retention_policy_is_keep_one()
        {
            var container = Container.Create("Container that keeps one revision", RevisionRetentionPolicyType.KeepOne);
            string initialValue = "this is my initial value";
            var textDocument = container.CreateTextDocument("Test document", _identity, initialValue);
            string newValue = initialValue + " and this is a revised value";
            textDocument.SetContent(newValue, _identity);

            Assert.IsTrue(textDocument.Content == newValue);
            Assert.IsTrue(textDocument.Revisions.Count == 1);
        }

        [TestMethod]
        public void Container_retains_one_revision_when_using_default_retention_policy()
        {
            var container = Container.Create("Container that keeps one revision");
            string initialValue = "this is my initial value";
            var textDocument = container.CreateTextDocument("Test document", _identity, initialValue);
            string newValue = initialValue + " and this is a revised value";
            textDocument.SetContent(newValue, _identity);

            Assert.IsTrue(textDocument.Content == newValue);
            Assert.IsTrue(textDocument.Revisions.Count == 1);
        }

        [TestMethod]
        public void Container_shows_correct_number_of_documents()
        {
            var container = Container.Create("Container that keeps one revision");
            string initialValue = "initial value for document 1";
            var textDocument1 = container.CreateTextDocument("Test document 1", _identity, initialValue);
            initialValue = "initial value for document 2";
            var textDocument2 = container.CreateTextDocument("Test document 2", _identity, initialValue);
            
            Assert.IsTrue(container.Documents.Count == 2);
            Assert.IsTrue(container.Documents.Where(s => s.Name == "Test document 1").SingleOrDefault() != null);
            Assert.IsTrue(container.Documents.Where(s => s.Name == "Test document 2").SingleOrDefault() != null);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Container_document_access_throws_if_not_unlocked()
        {
            var container = Container.Create("Container that keeps one revision");
            string initialValue = "initial value for document 1";
            var textDocument1 = container.CreateTextDocument("Test document 1", _identity, initialValue);
            container.Lock();
            Assert.IsTrue(container.Documents.Count == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Container_CreateTextDocument_throws_if_not_unlocked()
        {
            var container = Container.Create("Container that keeps one revision");
            container.Lock();
            string initialValue = "initial value for document 1";
            var textDocument1 = container.CreateTextDocument("Test document 1", _identity, initialValue);
            Assert.IsTrue(container.Documents.Count == 1);
        }
    }
}
