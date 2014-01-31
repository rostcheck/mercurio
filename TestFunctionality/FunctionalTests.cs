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
        private const string alicePassphrase = "Of all the queens, Alice is the highest!";
        private const string bobKey = "875DB1F1";
        private const string bobPassphrase = "Bob, just plain Bob, nothing to see here...";
        private const string aliceMessage = "Hi Bob. I'm Alice. Um... do you maybe want to hook up?";
        private const string bobMessage = "Hi Alice. Well, maybe. Can you send me some pics of you?";
        private Serializer serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
        private IPersistentQueue queue;
        private const string logFileName = "test.log";
        private static FileLogger logger = new FileLogger("test.log");
        private DummyMercurioUI userInterface = new DummyMercurioUI(logger);
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
            IMercurioMessage receivedMessage = bobMessageService.GetNext(bobAddress);
            Assert.IsTrue(receivedMessage.GetType() == typeof(ConnectInvitationMessage));
            IMercurioMessage response = bobMessageService.ProcessMessage(receivedMessage);
            bobMessageService.Send(response);

            // Sign in as Alice, receive accepted invitation
            TestUtils.SwitchUser(bobName, aliceName);
            receivedMessage = aliceMessageService.GetNext(aliceAddress);
            Assert.IsTrue(receivedMessage.GetType() == typeof(ConnectInvitationAcceptedMessage));
            var signedKeyMessage = aliceMessageService.ProcessMessage(receivedMessage); // Returns a signed copy of Bob's public key
            aliceMessageService.Send(signedKeyMessage); // Send signed key 
           
            // Alice also sends a message to Bob
            IMercurioMessage helloMessage = new SimpleTextMessage(aliceAddress, bobAddress, aliceMessage);
            aliceMessageService.Send(helloMessage);

            // Sign in as Bob, receive signed key
            TestUtils.SwitchUser(aliceName, bobName);
            receivedMessage = bobMessageService.GetNext(bobAddress);
            Assert.IsTrue(receivedMessage.GetType() == typeof(SignedKeyMessage));
            bobMessageService.ProcessMessage(receivedMessage);

            // Then receive message, reply
            receivedMessage = bobMessageService.GetNext(bobAddress);
            Assert.IsTrue(receivedMessage.GetType() == typeof(EncryptedMercurioMessage));
            Assert.IsFalse(receivedMessage.ToString() == aliceMessage.ToString());
            IMercurioMessage timeDelayedMessage = bobMessageService.ProcessMessage(receivedMessage);
            Assert.IsTrue(timeDelayedMessage.Content == aliceMessage.ToString());
            IMercurioMessage responseMessage = new SimpleTextMessage(bobAddress, aliceAddress, bobMessage);
            bobMessageService.Send(responseMessage);

            // Sign in as Alice, receive reply
            TestUtils.SwitchUser(bobName, aliceName);
            receivedMessage = aliceMessageService.GetNext(aliceAddress);
            Assert.IsTrue(receivedMessage.GetType() == typeof(EncryptedMercurioMessage));
            timeDelayedMessage = aliceMessageService.ProcessMessage(receivedMessage);
            Assert.IsFalse(receivedMessage.ToString() == bobMessage.ToString());
            Assert.IsTrue(timeDelayedMessage.Content == bobMessage);
        }

        //[TestMethod]
        //public void MakeBobQueue()
        //{
        //    const string sender = "bob@maker.net";
        //    const string recipient = "alice@maker.net";
        //    string[] messages = { "Hi Alice, are you there?", "I'm Bob, how are you?",
        //                        "Are you going to the party on Saturday?",
        //                        "I'm not bothering you, am I?", "I'm going, I think it will be fun."};
        //    Setup();
        //    TestUtils.SwitchUser(bobName, aliceName);
        //    foreach (string messageText in messages)
        //    {
        //        SimpleTextMessage message = new SimpleTextMessage(sender, recipient, messageText);
        //        bobMessageService.Send(message);
        //    }

        //}

        private void Setup()
        {
            foreach (string file in Directory.GetFiles(".", "*maker.net"))
                File.Delete(file);

            if (File.Exists(logFileName))
                File.Delete(logFileName);

            queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage, serializer);
            aliceConfig = TestConfig.GetTestConfiguration(aliceName);
            bobConfig = TestConfig.GetTestConfiguration(bobName);
            TestUtils.SetupUserDir(aliceName);
            TestUtils.SetupUserDir(bobName);
            aliceCryptoManager = CryptoManagerFactory.Create(CryptoManagerType.GPGCryptoManager, aliceConfig);
            aliceMessageService = new MessageService(queue, userInterface, aliceCryptoManager, serializer);
            aliceCryptoManager.SetPassphrase(alicePassphrase);
            bobCryptoManager = CryptoManagerFactory.Create(CryptoManagerType.GPGCryptoManager, bobConfig);
            bobCryptoManager.SetPassphrase(bobPassphrase);
            bobMessageService = new MessageService(queue, userInterface, bobCryptoManager, serializer);
        }

        private void ClearQueue(string address)
        {
            while (queue.Length(address) > 0) { queue.GetNext(address); } // clear queue
        }
    }
}
