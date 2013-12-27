using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Messages
{
    public class ConnectInvitationMessageProcessor : IMercurioMessageProcessor
    {
        public string MessageTypeName
        {
            get { return "Entities.ConnectInvitationMessage"; }
        }

        public IMercurioMessage ProcessMessage(IMercurioMessage message, ICryptoManager cryptoManager, IMercurioUI ui)
        {
            if (message.GetType() != typeof(ConnectInvitationMessage))
            {
                string messageTypeName = message.GetType().ToString();
                throw new Exception("Unknown message type " + message + " passed to ConnectInvitationMessageProcessor:ProcessMessage");
            }
            ConnectInvitationMessage thisMessage = message as ConnectInvitationMessage;
                
            string keyID = cryptoManager.ImportKey(thisMessage.PublicKey);
            string fingerprint = cryptoManager.GetFingerprint(keyID);
            if (ui.AcceptInvitation(thisMessage, fingerprint))
            {
                //if (cryptoManager.HasPublicKey(keyID))
                //{
                //    // Already have the key, nothing to do
                //    //TODO: log
                //    return;
                //}
                //else
                //{
                    cryptoManager.SignKey(keyID);
                    string signedKey = cryptoManager.GetPublicKey(keyID);
                    string userIdentity = ui.GetSelectedIdentity(cryptoManager);
                    var invitationAcceptedMessage = new ConnectInvitationAcceptedMessage(message.RecipientAddress,
                        message.SenderAddress, userIdentity, cryptoManager.GetPublicKey(userIdentity));

                   return invitationAcceptedMessage;
                //}
            }
            else
            {
                cryptoManager.DeleteKey(keyID); // Don't reply
                return null;
            }
        }
    }
}
