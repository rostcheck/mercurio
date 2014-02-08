using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using MercurioAppServiceLayer;

namespace TestFunctionality
{
    public class DummyUserAgent : IMercurioUserAgent
    {
        private List<string> outstandingInvitations;
        private string lastDisplayedMessage;
        private IMercurioLogger logger;
        private MessageService messageService;
        private ICryptoManager cryptoManager;

        public DummyUserAgent(IMercurioLogger logger, MessageService messageService, ICryptoManager cryptoManager)
        {
            if (logger == null)
                throw new ArgumentException("Must supply a valid logger");

            this.logger = logger;
            this.messageService = messageService;
            this.cryptoManager = cryptoManager;
            outstandingInvitations = new List<string>();
        }

        public void SetConfiguration(CryptoManagerConfiguration configuration)
        {
            cryptoManager.SetConfiguration(configuration);
            messageService.SetConfiguration(configuration);
        }

        public string LastDisplayedMessage
        {
            get
            {
                return lastDisplayedMessage;
            }
        }

        public string GetSelectedIdentity()
        {
           List<User> identityList = cryptoManager.GetAvailableIdentities();
            if (identityList.Count >= 1)
                return identityList[0].Identifier;
            else
                throw new Exception("No identities available");
        }

        public void DisplayMessage(IMercurioMessage message)
        {
            logger.Log(LogMessageLevelEnum.Normal, message.ToString());
            lastDisplayedMessage = message.ToString();
            // Automatically accept invitations
            Type messageType = message.GetType();
            if (messageType == typeof(ConnectInvitationMessage))
            {
                messageService.AcceptInvitation((ConnectInvitationMessage)message, GetSelectedIdentity());
            }
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
    }
}
