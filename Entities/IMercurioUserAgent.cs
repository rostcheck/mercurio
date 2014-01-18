using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Encapsulates interaction with the user
    /// </summary>
    public interface IMercurioUserAgent
    {
        void DisplayMessage(IMercurioMessage message, string senderAddress);
        string GetSelectedIdentity(ICryptoManager cryptoManager);
        bool AcceptInvitation(ConnectInvitationMessage invitationMessage, string fingerprint);
        bool AcceptInvitationResponse(ConnectInvitationAcceptedMessage invitationAcceptedMessage, string fingerprint);
        /// <summary>
        /// Got a message, can't deserialize it
        /// </summary>
        /// <param name="message">The message as an object</param>
        void InvalidMessageReceived(object message);
    }
}
