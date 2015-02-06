﻿using System;
using System.Collections.Generic;
using Entities;
using MercurioAppServiceLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtilities;
using Cryptography.GPG;
using Mercurio.Domain;
using Mercurio.Domain.Implementation;

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
            var configuration = TestUtilities.TestConfig.Create("Bob");
            CryptoManagerFactory.Register(CryptoType.GPG.ToString(), typeof(CrypographicServiceProviderGPG));
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoType.GPG.ToString());
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
