using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class MessageService
    {
        private IPersistentQueue queue;
        private IMercurioUI ui;
        private ICryptoManager cryptoManager;

        public void Send(IMercurioMessage message)
        {
            queue.Add(message);
        }

        public MessageService(IPersistentQueue queue, IMercurioUI userInterface, ICryptoManager cryptoManager)
        {
            this.queue = queue;
            this.ui = userInterface;
            this.cryptoManager = cryptoManager;
        }

        public void ProcessMessage(IMercurioMessage message)
        {
            string messageType = message.GetType().ToString();
            switch(messageType)
            {
                case "Entities.ConnectInvitationMessage":
                    ConnectInvitationMessage thisMessage = message as ConnectInvitationMessage;
                    string keyID = cryptoManager.ImportKey(thisMessage.PublicKey);
                    string fingerprint = cryptoManager.GetFingerprint(keyID);
                    if (ui.AcceptInvitation(thisMessage, fingerprint))
                    {

                    }
                    else
                    {
                        cryptoManager.DeleteKey(keyID);
                    }
                    break;
                default:
                    throw new Exception("Don't know how to process a message of type " + messageType);
            }
        }
       
    }
}
