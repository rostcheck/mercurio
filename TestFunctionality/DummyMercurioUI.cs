using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace TestFunctionality
{
    public class DummyMercurioUI : IMercurioUserAgent
    {
        private List<string> outstandingInvitations;
        private string lastDisplayedMessage;
        private IMercurioLogger logger;

        public DummyMercurioUI(IMercurioLogger logger)
        {
            if (logger == null)
                throw new ArgumentException("Must supply a valid logger");

            this.logger = logger;
            outstandingInvitations = new List<string>();
        }

        public string LastDisplayedMessage
        {
            get
            {
                return lastDisplayedMessage;
            }
        }

        public string GetSelectedIdentity(ICryptoManager cryptoManager)
        {
           List<User> identityList = cryptoManager.GetAvailableIdentities();
            if (identityList.Count == 1)
                return identityList[0].Identifier;
            else if (identityList.Count > 1)
                return identityList[0].Identifier;
            else
                throw new Exception("No identities available");
        }

        public bool AcceptInvitation(ConnectInvitationMessage invitationMessage, string fingerprint)
        {
            return true;
        }

        public bool AcceptInvitationResponse(ConnectInvitationAcceptedMessage invitationAcceptedMessage, string fingerprint)
        {
            return true;
        }

        public void DisplayMessage(IMercurioMessage message, string recipientAddress)
        {
            logger.Log(LogMessageLevelEnum.Normal, message.ToString());
            lastDisplayedMessage = message.ToString();
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
