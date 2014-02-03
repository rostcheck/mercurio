using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace MercurioAppServiceLayer
{
    public delegate void NewMessage(IMercurioMessage message, string senderAddress);

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
        private bool listening = false;
        private NewMessage newMessageEvent = null, replacedMessageEvent = null;

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
            Dictionary<ConfigurationKeyEnum, string> configuration = SetupConfiguration(cryptoManagerType);
            List<string> errorList = ValidateConfiguration(cryptoManagerType, configuration);
            if (errorList != null && errorList.Count > 0)
            {
                throw new Exception(errorList.ToString());
            }
            this.cryptoManager = CryptoManagerFactory.Create(GetCryptoManagerType(cryptoManagerType), configuration);
            this.logger = new FileLogger("mercurio_log.txt");
            queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage, serializer); // TODO: Make configurable
            this.messageService = new MessageService(queue, this, cryptoManager, serializer);
        }

        public async Task InjectTestMessages()
        {
            const string testMessageQueue = "messages_for_alice@maker.net";
            const string testMessageQueuePath = @"..\..\..\TestKeyRings\" + testMessageQueue;
            IPersistentQueue queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage, serializer);
            if (File.Exists(testMessageQueue))
                File.Delete(testMessageQueue);
            File.Copy(testMessageQueuePath, testMessageQueue);
            
            const int delay = 5000;
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
            Task.Run(() => InjectTestMessages());
            IMercurioMessage nextMessage = null;
            while (listening)
            {
                if (nextMessage == null)
                {
                    nextMessage = messageService.GetNext(listeningAddress);
                }
                if (nextMessage != null)
                {
                    nextMessage = messageService.ProcessMessage(nextMessage);
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

        public List<User> GetUsers()
        {
            return cryptoManager.GetAvailableUsers();
        }

        /// <summary>
        /// List of identities that the currently running user can operate as
        /// </summary>
        /// <returns>List of Users</returns>
        public List<User> GetAvailableIdentities()
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

        private Dictionary<ConfigurationKeyEnum, string> SetupConfiguration(AppCryptoManagerType appCryptoManagerType)
        {
            switch (appCryptoManagerType)
            {
                case AppCryptoManagerType.GPG:
                    return SetupGPGConfiguration();
                default:
                    throw new NotImplementedException("Unknown crypto manager type " + appCryptoManagerType.ToString() + " specified");
            }
        }

        private Dictionary<ConfigurationKeyEnum, string> SetupGPGConfiguration()
        {
            Dictionary<ConfigurationKeyEnum, string> configuration = new Dictionary<ConfigurationKeyEnum, string>();
            configuration[ConfigurationKeyEnum.UserHome] = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\gnupg";
            configuration[ConfigurationKeyEnum.GPGBinaryPath] = Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86) + "\\GNU\\GnuPG\\gpg2.exe";
            return configuration;
        }

        private CryptoManagerType GetCryptoManagerType(AppCryptoManagerType appCryptoManagerType)
        {
            switch (appCryptoManagerType)
            {
                case AppCryptoManagerType.GPG:
                    return CryptoManagerType.GPGCryptoManager;
                default:
                    throw new NotImplementedException("Unknown crypto manager type " + appCryptoManagerType.ToString() + " specified");
            }
        }
        
        private List<string> ValidateConfiguration(AppCryptoManagerType appCryptoManagerType, Dictionary<ConfigurationKeyEnum, string> config)
        {
            switch (appCryptoManagerType)
            {
                case AppCryptoManagerType.GPG:
                    return ValidateGPGConfiguration(config);
                default:
                    throw new NotImplementedException("Unknown crypto manager type " + appCryptoManagerType.ToString() + " specified");
            }
        }

        private List<string> ValidateGPGConfiguration(Dictionary<ConfigurationKeyEnum, string> config)
        {
            List<string> errorList = new List<string>();
            ConfigurationKeyEnum[] requiredKeys = { ConfigurationKeyEnum.UserHome, ConfigurationKeyEnum.UserHome };
            foreach (ConfigurationKeyEnum key in requiredKeys)
            {
                string error = ValidateConfigurationKey(AppCryptoManagerType.GPG, config, key);
                if (error != null && error != string.Empty)
                    errorList.Add(error);
            }
            return errorList;
        }

        private string ValidateConfigurationKey(AppCryptoManagerType appCryptoManagerType, 
            Dictionary<ConfigurationKeyEnum, string> config, 
            ConfigurationKeyEnum key)
        {
            string missingRequiredKey = "Configuration must contain key {0} for specified crypto manager type {1}";
            string missingRequiredValue = "Configuration key {0} does not contain a value - required for specified crypto manager type {1}";
            string errorString = null;
            if (config.Keys.Contains(key))
            {
                if (config[key] == null || config[key] == string.Empty)
                {
                    errorString = string.Format(missingRequiredValue, key.ToString(), appCryptoManagerType.ToString());
                }
            }
            else
            {
                errorString = string.Format(missingRequiredKey, key.ToString(), appCryptoManagerType.ToString());
            }
            return errorString;
        }

        #region IMercurioUserAgent

        public void DisplayMessage(IMercurioMessage message, string senderAddress)
        {
            if (messageStore.Store(message, senderAddress))
            {
                if (replacedMessageEvent != null)
                    ReplacedMessageEvent(message, senderAddress);
            }
            else
            {
                if (newMessageEvent != null)
                    NewMessageEvent(message, senderAddress);
            }
        }

        public string GetSelectedIdentity(ICryptoManager cryptoManager)
        {
            throw new NotImplementedException();
        }

        public bool AcceptInvitation(ConnectInvitationMessage invitationMessage, string fingerprint)
        {
            throw new NotImplementedException();
        }

        public bool AcceptInvitationResponse(ConnectInvitationAcceptedMessage invitationAcceptedMessage, string fingerprint)
        {
            throw new NotImplementedException();
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
