using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Mercurio
{
    public class AppServiceLayer
    {
        private ICryptoManager cryptoManager;
        private MessageService messageService;
        private IMercurioUI userInterface;
        private IPersistentQueue queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage); // TODO: Make configurable

        public AppServiceLayer(ICryptoManager cryptoManager, IMercurioUI userInterface)
        {
            this.cryptoManager = cryptoManager;
            this.userInterface = userInterface;
            this.messageService = new MessageService(queue, userInterface, cryptoManager);
        }

        public void SendInvitation(string userID, string address, string evidenceURL)
        {
            string publicKey = cryptoManager.GetPublicKey(userID);
            string[] signatures = cryptoManager.GetSignatures();
            ConnectInvitationMessage message = new ConnectInvitationMessage(address, publicKey, signatures, evidenceURL);
            messageService.Send(message);
        }
    }
}
