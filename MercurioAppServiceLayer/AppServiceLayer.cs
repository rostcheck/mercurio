using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Mercurio.Domain;
using Mercurio.Domain.Implementation;
using Cryptography.GPG;

namespace MercurioAppServiceLayer
{
    public delegate void NewMessage(IMercurioMessage message, string senderAddress);
    public delegate void NewInvitation(ConnectInvitationMessage message, string senderAddress);

    /// <summary>
    /// App service layer - provides all business logic, communicates in Entities. The app
    /// service layer should be common across different UIs on different platforms.
    /// </summary>
    public class AppServiceLayer : IMercurioUserAgent
    {
        private ICryptoManager cryptoManager;
        private MessageService messageService;
        private IMercurioLogger logger;
        private Serializer serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
        private IPersistentQueue queue;
        private MessageStoreTransientUnencrypted messageStore = new MessageStoreTransientUnencrypted();
        //TODO: persist this list
        private List<ConnectInvitationMessage> invitations = new List<ConnectInvitationMessage>();
        private bool listening = false;
        private NewMessage newMessageEvent, replacedMessageEvent;
        private NewInvitation newInvitationEvent;
        private string selectedIdentity;

        public NewInvitation NewInvitationEvent
        {
            get
            {
                return newInvitationEvent;
            }
            set
            {
                newInvitationEvent += value;
            }
        }

        public NewMessage NewMessageEvent
        {
            get
            {
                return newMessageEvent;
            }
            set
            {
                newMessageEvent += value;
            }
        }

        public NewMessage ReplacedMessageEvent
        {
            get
            {
                return replacedMessageEvent;
            }
            set
            {
                replacedMessageEvent += value;
            }
        }

        public IPersistentQueue MessageQueue
        {
            get
            {
                return queue;
            }
        }

        public AppServiceLayer(AppCryptoManagerType cryptoManagerType)
        {
            CryptoManagerConfiguration configuration = SetupConfiguration(cryptoManagerType);
            List<string> errorList = ValidateConfiguration(cryptoManagerType, configuration);
            if (errorList != null && errorList.Count > 0)
            {
                throw new Exception(errorList.ToString());
            }
            this.cryptoManager = CryptoManagerFactory.Create(GetCryptoManagerType(cryptoManagerType).ToString(), configuration);
            this.logger = new FileLogger("mercurio_log.txt");
            
            PersistentQueueConfiguration queueConfiguration = new PersistentQueueConfiguration(ConfigurationManager.AppSettings["StorageConnectionString"]);
            queue = PersistentQueueFactory.Create(PeristentQueueType.CloudQueueStorage, 
                queueConfiguration, serializer);
            this.messageService = new MessageService(queue, cryptoManager, serializer);
        }

        public async Task InjectTestMessages()
        {
            const string testMessageQueue = "messages_for_alice@maker.net";
            string testMessageQueuePath = Path.Combine(@"..", @"..", @"..", "TestKeyRings", testMessageQueue);
            PersistentQueueConfiguration queueConfiguration = new PersistentQueueConfiguration();
            IPersistentQueue queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage, queueConfiguration, serializer);
            if (File.Exists(testMessageQueue))
                File.Delete(testMessageQueue);
            File.Copy(testMessageQueuePath, testMessageQueue);
            
            const int delay = 2000;
            bool bExit = false;
            while (!bExit)
            {
                await Task.Delay(delay);
                EnvelopedMercurioMessage message = queue.GetNext(testMessageQueue);
                if (message == null)
                    bExit = true;
                else
                    queue.Add(message);
            }
        }

        public async Task StartListener(string listeningAddress, NetworkCredential credential)
        {
            const int delay = 500;
            listening = true;
            cryptoManager.SetCredential(credential);
            Task injectorTask = Task.Factory.StartNew(() => InjectTestMessages());
            IMercurioMessage nextMessage = null;
            while (listening)
            {
                if (nextMessage == null)
                {
                    nextMessage = messageService.GetNext(listeningAddress);
                }
                else
                {
                    // the result message may be intended for us (a chained message) or someone else
                    if (nextMessage.RecipientAddress == listeningAddress)
                    {
                        nextMessage.MessageIsDisplayableEvent += DisplayMessage;
                        nextMessage = messageService.ProcessMessage(nextMessage, selectedIdentity);
                    }
                    else
                    {
                        messageService.Send(nextMessage); // Pass it on
                    }
                }
                await Task.Delay(delay);
            }
        }

        public void StopListener()
        {
            listening = false;
            Task.Delay(600);
        }

        public void SendInvitation(string userID, string senderAddress, string recipientAddress, string evidenceURL)
        {
            string publicKey = cryptoManager.GetPublicKey(userID);
            string[] signatures = cryptoManager.GetSignatures();
            ConnectInvitationMessage message = new ConnectInvitationMessage(senderAddress, recipientAddress, publicKey, signatures, evidenceURL);
            messageService.Send(message);
        }

        public void AcceptInvitation(string userID, ConnectInvitationMessage invitation)
        {
            messageService.AcceptInvitation(invitation, userID);
        }

        public void RejectInvitation(ConnectInvitationMessage invitation)
        {
            messageService.RejectInvitation(invitation);
        }


        public void SendMessage(string senderAddress, string recipientAddress, string messageText)
        {
            SimpleTextMessage textMessage = new SimpleTextMessage(senderAddress, recipientAddress, messageText);
            messageService.Send(textMessage);
        }

        public List<ContactIdentity> GetContacts()
        {
            return cryptoManager.GetAvailableContactIdentities();
        }

        /// <summary>
        /// List of identities that the currently running user can operate as
        /// </summary>
        /// <returns>List of Users</returns>
        public List<UserIdentity> GetAvailableIdentities()
        {
            return cryptoManager.GetAvailableIdentities();
        }

        public string GetFingerprint(string userIdentifier)
        {
            return cryptoManager.GetFingerprint(userIdentifier);
        }

        public List<IMercurioMessage> GetMessages(string userAddress)
        {
            return messageStore.GetMessages(userAddress);
        }

        public List<ConnectInvitationMessage> GetInvitations()
        {
            return invitations;
        }

        public void SetSelectedIdentity(string identity)
        {
            selectedIdentity = identity;
        }

        private CryptoManagerConfiguration SetupConfiguration(AppCryptoManagerType appCryptoManagerType)
        {
            switch (appCryptoManagerType)
            {
                case AppCryptoManagerType.GPG:
                    return SetupGPGConfiguration();
                default:
                    throw new NotImplementedException("Unknown crypto manager type " + appCryptoManagerType.ToString() + " specified");
            }
        }

        private CryptoManagerConfiguration SetupGPGConfiguration()
        {
            CryptoManagerConfiguration configuration = new CryptoManagerConfiguration();
            configuration[GPGConfigurationKeyEnum.UserHome.ToString()] = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "gnupg");
            configuration[GPGConfigurationKeyEnum.GPGBinaryPath.ToString()] = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86), "GNU", "GnuPG", "gpg2.exe");
            return configuration;
        }

        private CryptoType GetCryptoManagerType(AppCryptoManagerType appCryptoManagerType)
        {
            switch (appCryptoManagerType)
            {
                case AppCryptoManagerType.GPG:
                    return CryptoType.GPG;
                default:
                    throw new NotImplementedException("Unknown crypto manager type " + appCryptoManagerType.ToString() + " specified");
            }
        }

        private List<string> ValidateConfiguration(AppCryptoManagerType appCryptoManagerType, CryptoManagerConfiguration config)
        {
            switch (appCryptoManagerType)
            {
                case AppCryptoManagerType.GPG:
                    return ValidateGPGConfiguration(config);
                default:
                    throw new NotImplementedException("Unknown crypto manager type " + appCryptoManagerType.ToString() + " specified");
            }
        }

        private List<string> ValidateGPGConfiguration(CryptoManagerConfiguration config)
        {
            List<string> errorList = new List<string>();
            GPGConfigurationKeyEnum[] requiredKeys = { GPGConfigurationKeyEnum.GPGBinaryPath, GPGConfigurationKeyEnum.UserHome };
            foreach (GPGConfigurationKeyEnum key in requiredKeys)
            {
                string error = ValidateConfigurationKey(AppCryptoManagerType.GPG, config, key);
                if (error != null && error != string.Empty)
                    errorList.Add(error);
            }
            return errorList;
        }

        private string ValidateConfigurationKey(AppCryptoManagerType appCryptoManagerType,
            CryptoManagerConfiguration config, GPGConfigurationKeyEnum keyEnum)
        {
            string missingRequiredKey = "Configuration must contain key {0} for specified crypto manager type {1}";
            string missingRequiredValue = "Configuration key {0} does not contain a value - required for specified crypto manager type {1}";
            string errorString = null;
            string key = keyEnum.ToString();
            if (config.Keys.Contains(key))
            {
                if (config[key] == null || config[key] == string.Empty)
                {
                    errorString = string.Format(missingRequiredValue, keyEnum.ToString(), appCryptoManagerType.ToString());
                }
            }
            else
            {
                errorString = string.Format(missingRequiredKey, keyEnum.ToString(), appCryptoManagerType.ToString());
            }
            return errorString;
        }

        public bool ValidateCredential(NetworkCredential credential)
        {
            return cryptoManager.ValidateCredential(credential);
        }

        #region IMercurioUserAgent

        public void DisplayMessage(IMercurioMessage message)
        {
            // Process invitations and accepted responses separately
            if (message.GetType() == typeof(ConnectInvitationMessage))
            {
                invitations.Add((ConnectInvitationMessage)message); //TODO: allows duplicates
                if (newInvitationEvent != null)
                    NewInvitationEvent((ConnectInvitationMessage)message, message.SenderAddress);
            }

            if (messageStore.Store(message, message.SenderAddress))
            {
                if (replacedMessageEvent != null)
                    ReplacedMessageEvent(message, message.SenderAddress);
            }
            else
            {
                if (newMessageEvent != null)
                    NewMessageEvent(message, message.SenderAddress);
            }
        }

        public string GetSelectedIdentity()
        {
            return selectedIdentity;
        }

        public void InvalidMessageReceived(object message)
        {
            IMercurioMessage mercurioMessage = message as IMercurioMessage;
            if (mercurioMessage == null)
            {
                string objectAsString = Convert.ToString(message);
                string firstPart = objectAsString.Substring(0, 25);
                string formatMessage = "Received invalid message - cannot be deserialized (starts with {0})";
                string logMessage = string.Format(formatMessage, firstPart);
                logger.Log(LogMessageLevelEnum.Normal, logMessage);
            }
            else
            {
                string formatMessage = "Received invalid message from {0} - cannot be deserialized (unknown message type {1})";
                string logMessage = string.Format(formatMessage, mercurioMessage.SenderAddress, mercurioMessage.GetType().ToString());
                logger.Log(LogMessageLevelEnum.Normal, logMessage);
            }
        }
        #endregion
    }
}
