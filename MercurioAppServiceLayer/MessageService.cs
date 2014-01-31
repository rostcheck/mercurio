using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using Entities;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace MercurioAppServiceLayer
{
    public class MessageService
    {
        private IPersistentQueue queue;
        private IMercurioUserAgent ui;
        private ICryptoManager cryptoManager;
        private Dictionary<string, IMercurioMessageProcessor> messageProcessors;
        private Serializer serializer;
       
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

        public MessageService(IPersistentQueue queue, IMercurioUserAgent userInterface, 
            ICryptoManager cryptoManager, Serializer serializer)
        {
            this.queue = queue;
            this.ui = userInterface;
            this.cryptoManager = cryptoManager;
            this.serializer = serializer;
            messageProcessors = LoadMessageProcessors();
        }

        private Dictionary<string, IMercurioMessageProcessor> LoadMessageProcessors()
        {
            // This could be made discoverable, but not sure that's a good idea from security
            // perspective (too easy to allow injection of altered processors?)
            var processors = new Dictionary<string, IMercurioMessageProcessor>();
            processors[typeof(ConnectInvitationMessage).ToString()] = new ConnectInvitationMessageProcessor();
            processors[typeof(ConnectInvitationAcceptedMessage).ToString()] = new ConnectInvitationAcceptedMessageProcessor();
            processors[typeof(SignedKeyMessage).ToString()] = new SignedKeyMessageProcessor();
            processors[typeof(SimpleTextMessage).ToString()] = new SimpleTextMessageProcessor();
            processors[typeof(EncryptedMercurioMessage).ToString()] = new EncryptedMessageProcessor();
            processors[typeof(DelayedMessage).ToString()] = new DelayedMessageProcessor();
            return processors;
        }

        public IMercurioMessage ProcessMessage(IMercurioMessage message)
        {
            string messageType = message.GetType().ToString();
            if (messageProcessors.ContainsKey(messageType))
            {
                return messageProcessors[messageType].ProcessMessage(message, cryptoManager, ui, serializer);
            }
            else
            {
                throw new Exception("Don't know how to process a message of type " + messageType);
            }
        }       
    }
}
