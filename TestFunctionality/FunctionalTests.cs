using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Entities;
using System.Collections.Generic;
using TestUtilities;
using TestEntities;
using MercurioAppServiceLayer;

namespace TestFunctionality
{
    [TestClass]
    public class FunctionalTests
    {
        private const string aliceName = "Alice";
        private const string bobName = "Bob";
        private const string aliceAddress = "alice@maker.net";
        private const string bobAddress = "bob@maker.net";
        private const string evidenceURL = "http://thisisdavidr.net/pgp_fingerprint.m4v";
        private const string aliceKey = "B20A4563";
        private const string bobKey = "875DB1F1";
        private const string bobPassphrase = "Bob, just plain Bob, nothing to see here...";
        private IPersistentQueue queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage);
        private IMercurioUI userInterface = new DummyMercurioUI();
        private Dictionary<ConfigurationKeyEnum, string> aliceConfig, bobConfig;
        private ICryptoManager aliceCryptoManager, bobCryptoManager;
        private MessageService aliceMessageService, bobMessageService;

        [TestMethod]
        public void KeyExchange()
        {
            Setup();

            // Sign in as Alice and send an invitation to Bob
            TestUtils.SwitchUser(null, aliceName);
            ClearQueue(bobAddress);

            string[] signatures = new string[0];
            IMercurioMessage connectInvitationMessage = new ConnectInvitationMessage(aliceAddress, bobAddress, aliceCryptoManager.GetPublicKey(aliceKey), signatures, evidenceURL);
            aliceMessageService.Send(connectInvitationMessage);

            // Sign in as Bob, accept invitation
            TestUtils.SwitchUser(aliceName, bobName);
            IMercurioMessage receivedMessage = queue.GetNext(bobAddress);
            Assert.IsTrue(receivedMessage.GetType() == typeof(ConnectInvitationMessage));
            bobMessageService.ProcessMessage(receivedMessage);


            // Sign in as Alice, receive accepted invitation

            // Alice sends a message to Bob

            // Sign in as Bob, receive message, reply

            // Sign in as Alice, receive reply
        }

        private void Setup()
        {
            aliceConfig = TestConfig.GetTestConfiguration(aliceName);
            bobConfig = TestConfig.GetTestConfiguration(bobName);
            TestUtils.SetupUserDir(aliceName);
            TestUtils.SetupUserDir(bobName);
            aliceCryptoManager = CryptoManagerFactory.Create(CryptoManagerType.GPGCryptoManager, aliceConfig);
            aliceMessageService = new MessageService(queue, userInterface, aliceCryptoManager);
            bobCryptoManager = CryptoManagerFactory.Create(CryptoManagerType.GPGCryptoManager, bobConfig);
            bobCryptoManager.SetPassphrase(bobPassphrase);
            bobMessageService = new MessageService(queue, userInterface, bobCryptoManager);
        }

        private void ClearQueue(string address)
        {
            while (queue.Length(address) > 0) { queue.GetNext(address); } // clear queue
        }
    }
}
