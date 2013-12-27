using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using Entities;

namespace MercurioAppServiceLayer
{
    public class MessageService
    {
        private IPersistentQueue queue;
        private IMercurioUI ui;
        private ICryptoManager cryptoManager;
        private Dictionary<string, IMercurioMessageProcessor> messageProcessors;
       
        public void Send(IMercurioMessage message)
        {
            queue.Add(message);
        }

        public MessageService(IPersistentQueue queue, IMercurioUI userInterface, ICryptoManager cryptoManager)
        {
            this.queue = queue;
            this.ui = userInterface;
            this.cryptoManager = cryptoManager;
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
            return processors;
        }

        public IMercurioMessage ProcessMessage(IMercurioMessage message)
        {
            string messageType = message.GetType().ToString();
            if (messageProcessors.ContainsKey(messageType))
            {
                return messageProcessors[messageType].ProcessMessage(message, cryptoManager, ui);
            }
            else
            {
                throw new Exception("Don't know how to process a message of type " + messageType);
            }
        }       
    }
}
