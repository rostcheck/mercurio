using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace MercurioAppServiceLayer
{
    /// <summary>
    /// Encapsulates communication between app service (user agent) and its UI
    /// </summary>
    public interface IMercurioUI
    {
        void NewMessage(IMercurioMessage message, string senderAddress);
        string GetSelectedIdentity(ICryptoManager cryptoManager);
        bool AcceptInvitation(ConnectInvitationMessage invitationMessage, string fingerprint);
        bool AcceptInvitationResponse(ConnectInvitationAcceptedMessage invitationAcceptedMessage, string fingerprint);
        void InvalidMessageReceived(object message);
    }
}
