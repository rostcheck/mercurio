using System;
using System.Collections.Generic;
using Entities;
using MercurioAppServiceLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtilities;

namespace TestFunctionality
{
    [TestClass]
    public class MessageServiceTest
    {
        private const string senderAddress = "alice@maker.net";
        private const string recipientAddress = "bob@maker.net";
        private const string evidenceURL = "http://thisisdavidr.net/pgp_fingerprint.m4v";
        private const string senderKey = "79222C24";

        [TestMethod]
        public void ConnectInvitationTest()
        {
            Dictionary <ConfigurationKeyEnum, string> configuration = TestUtilities.TestConfig.Create("Bob");
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoManagerType.GPGCryptoManager, configuration);
            Serializer serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            PersistentQueueConfiguration queueConfiguration = new PersistentQueueConfiguration();
            IPersistentQueue queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage, queueConfiguration, serializer);
            FileLogger logger = new FileLogger("test.log");
            MessageService messageService = new MessageService(queue, cryptoManager, serializer);
            IMercurioUserAgent userInterface = new DummyUserAgent(logger, messageService, cryptoManager);
            string[] signatures = new string[0];
            IMercurioMessage connectInvitationMessage = new ConnectInvitationMessage(senderAddress, recipientAddress, cryptoManager.GetPublicKey(senderKey), signatures, evidenceURL);
            messageService.Send(connectInvitationMessage);
        }
    }
}
