using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Entities;
using System.Collections.Generic;
using TestUtilities;
using TestEntities;
using MercurioAppServiceLayer;
using System.Net;

namespace TestFunctionality
{
    public class TestUserInfo
    {
        public string Name { get; set; }
        public string Address;
        public string Key;
        public string Passphrase;
    }

    [TestClass]
    public class FunctionalTests
    {
        private Dictionary<string, TestUserInfo> user = new Dictionary<string, TestUserInfo>()
        {
            { "Alice", new TestUserInfo { Name="Alice", Address="alice@maker.net", Key="B20A4563", Passphrase="Of all the queens, Alice is the highest!" }},
            { "Bob", new TestUserInfo { Name="Bob", Address="bob@maker.net", Key="875DB1F1", Passphrase = "Bob, just plain Bob, nothing to see here..." }},
            { "Carlos", new TestUserInfo { Name="Carlos", Address="carlos@maker.net", Key="4729F5D5", Passphrase="They will never guess my secret key!"}},
            { "Danielle", new TestUserInfo { Name="Danielle", Address="danielle@secret.net", Key="4FF770D2", Passphrase="Todos necesitan una forma de vida secreta, no es?" }}
        };
        private const string alice = "Alice";
        private const string bob = "Bob";
        private const string carlos = "Carlos";
        private const string danielle = "Danielle";
        private const string evidenceURL = "http://thisisdavidr.net/pgp_fingerprint.m4v";
        private const string aliceMessage = "Hi Bob. I'm Alice. Um... do you maybe want to hook up?";
        private const string bobMessage = "Hi Alice. Well, maybe. Can you send me some pics of you?";       
        private Serializer serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
        private IPersistentQueue queue;
        private const string logFileName = "test.log";
        private static FileLogger logger = new FileLogger("test.log");
        private ICryptoManager cryptoManager;
        private MessageService messageService;
        private NetworkCredential aliceCredential, bobCredential;
        DummyUserAgent userAgent;

        [TestMethod]
        public void KeyExchange()
        {
            Setup(alice);

            // Sign in as Alice and send an invitation to Bob
            ClearQueue(user[bob].Address);

            string[] signatures = new string[0];
            IMercurioMessage connectInvitationMessage = new ConnectInvitationMessage(user[alice].Address, user[bob].Address, cryptoManager.GetPublicKey(user[alice].Key), signatures, evidenceURL);
            messageService.Send(connectInvitationMessage);

            // Sign in as Bob, accept invitation
            SwitchUser(alice, bob);
            IMercurioMessage receivedMessage = messageService.GetNext(user[bob].Address);
            Assert.IsTrue(receivedMessage.GetType() == typeof(ConnectInvitationMessage));
            receivedMessage = messageService.ProcessMessage(receivedMessage, userAgent.GetSelectedIdentity());
            // Displaying the message will cause the dummy user agent to automatically
            // accept it and send a response
            userAgent.DisplayMessage(receivedMessage);

            // Sign in as Alice, receive accepted invitation
            SwitchUser(bob, alice);
            receivedMessage = messageService.GetNext(user[alice].Address);
            Assert.IsTrue(receivedMessage.GetType() == typeof(ConnectInvitationAcceptedMessage));
            // Returns a signed copy of Bob's public key
            var signedKeyMessage = messageService.ProcessMessage(receivedMessage, userAgent.GetSelectedIdentity()); 
            messageService.Send(signedKeyMessage); // Send signed key 
           
            // Alice also sends a message to Bob
            IMercurioMessage helloMessage = new SimpleTextMessage(user[alice].Address, user[bob].Address, aliceMessage);
            messageService.Send(helloMessage);

            // Sign in as Bob, receive signed key
            SwitchUser(alice, bob);
            receivedMessage = messageService.GetNext(user[bob].Address);
            Assert.IsTrue(receivedMessage.GetType() == typeof(SignedKeyMessage));
            messageService.ProcessMessage(receivedMessage, userAgent.GetSelectedIdentity());

            // Then receive message, reply
            receivedMessage = messageService.GetNext(user[bob].Address);
            Assert.IsTrue(receivedMessage.GetType() == typeof(EncryptedMercurioMessage));
            Assert.IsFalse(receivedMessage.ToString() == aliceMessage.ToString());
            IMercurioMessage timeDelayedMessage = messageService.ProcessMessage(receivedMessage, userAgent.GetSelectedIdentity());
            Assert.IsTrue(timeDelayedMessage.Content == aliceMessage.ToString());
            IMercurioMessage responseMessage = new SimpleTextMessage(user[bob].Address, user[alice].Address, bobMessage);
            messageService.Send(responseMessage);

            // Sign in as Alice, receive reply
            SwitchUser(bob, alice);
            receivedMessage = messageService.GetNext(user[alice].Address);
            Assert.IsTrue(receivedMessage.GetType() == typeof(EncryptedMercurioMessage));
            timeDelayedMessage = messageService.ProcessMessage(receivedMessage, userAgent.GetSelectedIdentity());
            Assert.IsFalse(receivedMessage.ToString() == bobMessage.ToString());
            Assert.IsTrue(timeDelayedMessage.Content == bobMessage);
        }

        [TestMethod]
        public void MakeTestQueue()
        {
            const string sender = "bob@maker.net";
            const string recipient = "alice@maker.net";
            string[] messages = { "Hi Alice, are you there?", "I'm Bob, how are you?",
                                "Are you going to the party on Saturday?",
                                "I'm not bothering you, am I?", "I'm going, I think it will be fun."};
            // Sign in as Alice and send an invitation to Bob
            Setup(alice);
            ClearQueue(user[bob].Address);

            string[] signatures = new string[0];
            IMercurioMessage connectInvitationMessage = new ConnectInvitationMessage(user[alice].Address, user[bob].Address, cryptoManager.GetPublicKey(user[alice].Key), signatures, evidenceURL);
            messageService.Send(connectInvitationMessage);

            // Sign in as Bob, accept invitation
            SwitchUser(alice, bob);
            IMercurioMessage receivedMessage = messageService.GetNext(user[bob].Address);
            Assert.IsTrue(receivedMessage.GetType() == typeof(ConnectInvitationMessage));
            IMercurioMessage response = messageService.ProcessMessage(receivedMessage, userAgent.GetSelectedIdentity());
            messageService.Send(response);

            // Send many messages
            foreach (string messageText in messages)
            {
                SimpleTextMessage message = new SimpleTextMessage(sender, recipient, messageText);
                messageService.Send(message);
            }

            // Queue up more invitations for Alice from Carlos and Danielle
            SwitchUser(bob, carlos);
            connectInvitationMessage = new ConnectInvitationMessage(user[carlos].Address, user[alice].Address, cryptoManager.GetPublicKey(user[alice].Key), signatures, evidenceURL);
            messageService.Send(connectInvitationMessage);
            SwitchUser(carlos, danielle);
            connectInvitationMessage = new ConnectInvitationMessage(user[danielle].Address, user[alice].Address, cryptoManager.GetPublicKey(user[alice].Key), signatures, evidenceURL);
            messageService.Send(connectInvitationMessage);
        }

        private void Setup(string userName)
        {
            foreach (string file in Directory.GetFiles(".", "*.net"))
                File.Delete(file);

            if (File.Exists(logFileName))
                File.Delete(logFileName);

            queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage, serializer);
            TestUtils.SetupUserDir(alice);
            TestUtils.SetupUserDir(bob);
            TestUtils.SetupUserDir(carlos);
            TestUtils.SetupUserDir(danielle);

            aliceCredential = new NetworkCredential(user[alice].Key, user[alice].Passphrase);
            bobCredential = new NetworkCredential(user[bob].Key, user[bob].Passphrase);
            cryptoManager = CryptoManagerFactory.Create(CryptoManagerType.GPGCryptoManager,
                TestConfig.GetTestConfiguration(userName));
            cryptoManager.SetCredential(GetCredential(userName));
            messageService = new MessageService(queue, cryptoManager, serializer);
            userAgent = new DummyUserAgent(logger, messageService, cryptoManager);
            SwitchUser(null, userName);
        }

        private void ClearQueue(string address)
        {
            while (queue.Length(address) > 0) { queue.GetNext(address); } // clear queue
        }

        private NetworkCredential GetCredential(string username)
        {
            return new NetworkCredential(username, user[username].Passphrase);
        }

        private void SwitchUser(string fromUser, string toUser)
        {
            TestUtils.SwitchUser(fromUser, toUser);
            CryptoManagerConfiguration configuration = TestConfig.GetTestConfiguration(toUser);
            messageService.SetConfiguration(configuration);
            cryptoManager.SetConfiguration(configuration);
            cryptoManager.SetCredential(GetCredential(toUser));
        }
    }
}
