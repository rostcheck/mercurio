using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace MercurioAppServiceLayer
{
    public class AppServiceLayer : IMercurioUserAgent
    {
        private ICryptoManager cryptoManager;
        private MessageService messageService;
        private IMercurioUI userInterface;
        private IMercurioLogger logger;
        private Serializer serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
        private IPersistentQueue queue;

        public IPersistentQueue MessageQueue
        {
            get
            {
                return queue;
            }
        }

        public AppServiceLayer(AppCryptoManagerType cryptoManagerType, IMercurioUI userInterface)
        {
            Dictionary<ConfigurationKeyEnum, string> configuration = SetupConfiguration(cryptoManagerType);
            List<string> errorList = ValidateConfiguration(cryptoManagerType, configuration);
            if (errorList != null && errorList.Count > 0)
            {
                throw new Exception(errorList.ToString());
            }
            this.cryptoManager = CryptoManagerFactory.Create(GetCryptoManagerType(cryptoManagerType), configuration);
            this.userInterface = userInterface;
            this.logger = new FileLogger("mercurio_log.txt");
            queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage, serializer); // TODO: Make configurable
            this.messageService = new MessageService(queue, this, cryptoManager, serializer);
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

        public User GetIdentity()
        {
            return cryptoManager.GetAvailableIdentities().FirstOrDefault<User>(); // TODO: generalize this
        }

        public List<string> GetMessages(string identity)
        {
            //TODO: implement for real
            return new List<string>();
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

        #region IMercurioUI
        public void DisplayTextMessage(string textMessage)
        {
            throw new NotImplementedException();
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
