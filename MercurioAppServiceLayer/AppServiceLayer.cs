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
        private IPersistentQueue queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage); // TODO: Make configurable

        public AppServiceLayer(ICryptoManager cryptoManager)
        {
            this.cryptoManager = cryptoManager;
            this.messageService = new MessageService(queue);
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
