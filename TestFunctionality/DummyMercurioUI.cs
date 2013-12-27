using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace TestFunctionality
{
    public class DummyMercurioUI : IMercurioUI
    {
        private List<string> outstandingInvitations;
        private string lastDisplayedMessage;

        public DummyMercurioUI()
        {
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
            Identity[] identityList = cryptoManager.GetAvailableIdentities();
            if (identityList.Length == 1)
                return identityList[0].Identifier;
            else if (identityList.Length > 1)
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

        public void DisplayTextMessage(string textMessage)
        {
            lastDisplayedMessage = textMessage;
        }
    }
}
