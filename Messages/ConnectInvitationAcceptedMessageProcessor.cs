using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Messages
{
    public class ConnectInvitationAcceptedMessageProcessor : IMercurioMessageProcessor
    {
        public string MessageTypeName
        {
            get { return "Entities.ConnectInvitationAcceptedMessage"; }
        }

        public IMercurioMessage ProcessMessage(IMercurioMessage message, ICryptoManager cryptoManager, IMercurioUI ui)
        {
            if (message.GetType() != typeof(ConnectInvitationAcceptedMessage))
            {
                string messageTypeName = message.GetType().ToString();
                throw new Exception("Unknown message type " + message + " passed to ConnectInvitationAcceptedMessageProcessor:ProcessMessage");
            }
            ConnectInvitationAcceptedMessage thisMessage = message as ConnectInvitationAcceptedMessage;

            // TODO: Is this message expected? Secure protocol more

            string keyID = cryptoManager.ImportKey(thisMessage.SignedPublicKey);
            string fingerprint = cryptoManager.GetFingerprint(keyID);
            if (ui.AcceptInvitationResponse(thisMessage, fingerprint))
            {
                // Countersign the key and send it back
                cryptoManager.SignKey(keyID);
                string signedKey = cryptoManager.GetPublicKey(keyID);
                string userIdentity = ui.GetSelectedIdentity(cryptoManager);
                // Reverse sender and recipient
                string sender = message.RecipientAddress;
                string recipient = message.SenderAddress;
                var signedKeyMessage = new SignedKeyMessage(recipient, sender, 
                   cryptoManager.GetPublicKey(userIdentity), string.Empty);
                return signedKeyMessage;
            }
            else
            {
                cryptoManager.DeleteKey(keyID); // Forget it, don't trust you
                return null;
            }
        }
    }
}
