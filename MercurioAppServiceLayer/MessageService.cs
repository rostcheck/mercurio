using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace MercurioAppServiceLayer
{
    /// <summary>
    /// Handles interactions with messages
    /// </summary>
    public class MessageService
    {
        private IPersistentQueue queue;
        private ICryptoManager cryptoManager;
        private Serializer serializer;
       
        public string GetPublicKey(string identifier)
        {
            return cryptoManager.GetPublicKey(identifier);
        }

        public void Send(IMercurioMessage message)
        {
            IMercurioMessage sendableMessage = message;
            if (message.Encryptable)
            {
                sendableMessage = new EncryptedMercurioMessage(cryptoManager, serializer, message);
            }

            EnvelopedMercurioMessage envelopedMessage = new EnvelopedMercurioMessage(
                message.SenderAddress, message.RecipientAddress, sendableMessage, serializer);

            queue.Add(envelopedMessage);
        }

        public IMercurioMessage GetNext(string address)
        {
            object rawMessage = queue.GetNext(address);
            EnvelopedMercurioMessage envelopedMessage = rawMessage as EnvelopedMercurioMessage;
            if (envelopedMessage == null)
            {
                //ui.InvalidMessageReceived(rawMessage);
                return null;
            }
            else
            {
                IMercurioMessage message = envelopedMessage.PayloadAsMessage(serializer);
                if (message.GetType() == typeof(EncryptedMercurioMessage))
                {
                    return (EncryptedMercurioMessage)message;
                }
                else return message;
            }
        }

        public MessageService(IPersistentQueue queue, ICryptoManager cryptoManager, Serializer serializer)
        {
            this.queue = queue;
            this.cryptoManager = cryptoManager;
            this.serializer = serializer;
        }

        public void SetConfiguration(CryptoManagerConfiguration configuration)
        {
            cryptoManager.SetConfiguration(configuration);
        }

        public IMercurioMessage ProcessMessage(IMercurioMessage message, string userIdentity)
        {
            return message.Process(cryptoManager, serializer, userIdentity);
        }

        public void AcceptInvitation(ConnectInvitationMessage invitation, string userIdentity)
        {
            if (invitation == null)
                throw new ArgumentException("Null invitation passed to AcceptInvitation");

            cryptoManager.SignKey(invitation.KeyID);
            string signedKey = cryptoManager.GetPublicKey(invitation.KeyID);
            // Reverse the sender and recipient, send an invitation accepted message back
            string senderAddress = invitation.RecipientAddress;
            string recipientAddress = invitation.SenderAddress;
            var invitationAcceptedMessage = new ConnectInvitationAcceptedMessage(senderAddress,
                recipientAddress, userIdentity, cryptoManager.GetPublicKey(userIdentity));

            Send(invitationAcceptedMessage);
        }

        public void RejectInvitation(ConnectInvitationMessage invitation)
        {
            if (invitation == null)
                throw new ArgumentException("Null invitation passed to RejectInvitation");

            cryptoManager.DeleteKey(invitation.KeyID); // Don't reply
        }
    }
}
