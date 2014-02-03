﻿using System;
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
            Dictionary <ConfigurationKeyEnum, string> configuration = TestUtilities.TestConfig.Create();
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoManagerType.GPGCryptoManager, configuration);
            Serializer serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            IPersistentQueue queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage, serializer);
            FileLogger logger = new FileLogger("test.log");
            IMercurioUserAgent userInterface = new DummyMercurioUI(logger); 
            MessageService messageService = new MessageService(queue, userInterface, cryptoManager, serializer);
            string[] signatures = new string[0];
            IMercurioMessage connectInvitationMessage = new ConnectInvitationMessage(senderAddress, recipientAddress, cryptoManager.GetPublicKey(senderKey), signatures, evidenceURL);
            messageService.Send(connectInvitationMessage);
        }
    }
}