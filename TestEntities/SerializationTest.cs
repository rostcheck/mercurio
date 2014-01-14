﻿using System;
using System.Collections.Generic;
using System.IO;
using Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtilities;

namespace TestEntities
{
    [TestClass]
    public class SerializationTest
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
        private IPersistentQueue queue;
        private Serializer serializer;

        private static FileLogger logger = new FileLogger("test.log");
        //private DummyMercurioUI userInterface = new DummyMercurioUI(logger);
        private Dictionary<ConfigurationKeyEnum, string> aliceConfig, bobConfig;
        private ICryptoManager aliceCryptoManager, bobCryptoManager;
        //private MessageService aliceMessageService, bobMessageService;

        [TestMethod]
        public void BasicSerialization()
        {
            Setup();
            TestUtils.SwitchUser(null, aliceName);

            string filename = "connectInvitationMessage.txt";
            string[] signatures = new string[0];
            IMercurioMessage connectInvitationMessage = new ConnectInvitationMessage(aliceAddress, bobAddress, aliceCryptoManager.GetPublicKey(aliceKey), signatures, evidenceURL);
            serializer.Serialize(filename, connectInvitationMessage);

            IMercurioMessage recoveredMessage = serializer.Deserialize<IMercurioMessage>(filename);
            ConnectInvitationMessage originalInvite = connectInvitationMessage as ConnectInvitationMessage;
            ConnectInvitationMessage recoveredInvite = recoveredMessage as ConnectInvitationMessage;
            Assert.IsTrue(originalInvite != null);
            Assert.IsTrue(recoveredInvite != null);
            Assert.IsTrue(originalInvite.PublicKey == recoveredInvite.PublicKey);
        }

        [TestMethod]
        public void EnvelopedSerialization()
        {
            Setup();
            TestUtils.SwitchUser(null, aliceName);

            string filename = "envelopedMessage.txt";
            string[] signatures = new string[0];
            IMercurioMessage connectInvitationMessage = new ConnectInvitationMessage(aliceAddress, bobAddress, aliceCryptoManager.GetPublicKey(aliceKey), signatures, evidenceURL);
            EnvelopedMercurioMessage envelopedMessage = new EnvelopedMercurioMessage(aliceAddress, bobAddress, connectInvitationMessage, serializer);
            serializer.Serialize(filename, envelopedMessage);

            EnvelopedMercurioMessage recoveredEnvelopedMessage = serializer.Deserialize<EnvelopedMercurioMessage>(filename);
            Assert.IsTrue(recoveredEnvelopedMessage != null);
            Assert.IsTrue(recoveredEnvelopedMessage.Payload != null);
            IMercurioMessage recoveredPayloadMessage = recoveredEnvelopedMessage.PayloadAsMessage(serializer);
            ConnectInvitationMessage recoveredInvitationMessage = recoveredPayloadMessage as ConnectInvitationMessage;
            Assert.IsTrue(recoveredInvitationMessage != null);
            Assert.IsTrue(recoveredInvitationMessage.PublicKey == ((ConnectInvitationMessage)connectInvitationMessage).PublicKey);

        }

        [TestMethod]
        public void SerializationA()
        {
            Setup();
            TestUtils.SwitchUser(null, aliceName);

            A a = new A();
            serializer.Serialize("a.txt", a);
            Assert.IsTrue(true);
        }


        private void Setup()
        {
            serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage, serializer);

            foreach (string file in Directory.GetFiles(".", "*maker.net"))
                File.Delete(file);

            queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage, serializer);
            aliceConfig = TestConfig.GetTestConfiguration(aliceName);
            bobConfig = TestConfig.GetTestConfiguration(bobName);
            TestUtils.SetupUserDir(aliceName);
            TestUtils.SetupUserDir(bobName);
            aliceCryptoManager = CryptoManagerFactory.Create(CryptoManagerType.GPGCryptoManager, aliceConfig);
            //aliceMessageService = new MessageService(queue, userInterface, aliceCryptoManager);
            aliceCryptoManager.SetPassphrase(alicePassphrase);
            bobCryptoManager = CryptoManagerFactory.Create(CryptoManagerType.GPGCryptoManager, bobConfig);
            bobCryptoManager.SetPassphrase(bobPassphrase);
            //bobMessageService = new MessageService(queue, userInterface, bobCryptoManager);        
        }
    }
}
