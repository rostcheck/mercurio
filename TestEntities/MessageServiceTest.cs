using System;
using System.Collections.Generic;
using Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestEntities
{
    [TestClass]
    public class MessageServiceTest
    {
        private const string recipientAddress = "bob@maker.net";
        private const string evidenceURL = "http://thisisdavidr.net/pgp_fingerprint.m4v";
        private const string senderKey = "79222C24";

        [TestMethod]
        public void ConnectInvitationTest()
        {
            Dictionary <ConfigurationKeyEnum, string> configuration = TestConfiguration.Create();
            ICryptoManager cryptoManager = CryptoManagerFactory.Create(CryptoManagerType.GPGCryptoManager, configuration);
            IPersistentQueue queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage);
            MessageService messageService = new MessageService(queue);
            string[] signatures = new string[0];
            IMercurioMessage connectInvitationMessage = new ConnectInvitationMessage(recipientAddress, cryptoManager.GetPublicKey(senderKey), signatures, evidenceURL);
            messageService.Send(connectInvitationMessage);
        }
    }
}
