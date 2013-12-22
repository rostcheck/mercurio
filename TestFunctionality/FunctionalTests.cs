using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Entities;
using System.Collections.Generic;
using TestUtils;
using TestEntities;

namespace TestFunctionality
{
    [TestClass]
    public class FunctionalTests
    {
        private const string recipientAddress = "bob@maker.net";
        private const string evidenceURL = "http://thisisdavidr.net/pgp_fingerprint.m4v";
        private const string alicesKey = "B20A4563";
        private const string bobsKey = "57739AE6";

        [TestMethod]
        public void KeyExchange()
        {
            // Initialize to test key rings
            string directory = Directory.GetCurrentDirectory();
            //string[] users = { "Alice", "Bob" };
            TestUtils.TestUtils.SetupUserDir("Alice");

            // Sign in as Alice and send an invitation to Bob
            Dictionary<ConfigurationKeyEnum, string> aliceConfig = TestConfiguration1.GetTestConfiguration("Alice");
            ICryptoManager aliceCryptoManager = CryptoManagerFactory.Create(CryptoManagerType.GPGCryptoManager, aliceConfig);
            IPersistentQueue queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage);
            IMercurioUI userInterface = new DummyMercurioUI();
            while (queue.Length(recipientAddress) > 0) { queue.GetNext(recipientAddress); } // clear queue
            MessageService messageService = new MessageService(queue, userInterface, aliceCryptoManager);            
            string[] signatures = new string[0];
            IMercurioMessage connectInvitationMessage = new ConnectInvitationMessage(recipientAddress, aliceCryptoManager.GetPublicKey(alicesKey), signatures, evidenceURL);
            messageService.Send(connectInvitationMessage);

            // Sign in as Bob, accept invitation
            Dictionary<ConfigurationKeyEnum, string> bobConfig = TestConfiguration1.GetTestConfiguration("Bob");
            ICryptoManager bobCryptoManager = CryptoManagerFactory.Create(CryptoManagerType.GPGCryptoManager, bobConfig);
            IMercurioMessage receivedMessage = queue.GetNext(recipientAddress);
            Assert.IsTrue(receivedMessage.GetType() == typeof(ConnectInvitationMessage));
            //messageService


            // Sign in as Alice, receive accepted invitation

            // Alice sends a message to Bob

            // Sign in as Bob, receive message, reply

            // Sign in as Alice, receive reply
        }






    }
}
