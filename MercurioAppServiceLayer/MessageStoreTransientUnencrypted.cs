using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Mercurio.Domain;

namespace MercurioAppServiceLayer
{
    /// <summary>
    /// Volatile message store - stores messages in memory only; they disappear when the store does
    /// </summary>
    public class MessageStoreTransientUnencrypted : IMessageStore
    {
        private Dictionary<string, List<IMercurioMessage>> messageStore;

        public MessageStoreTransientUnencrypted()
        {
            messageStore = new Dictionary<string, List<IMercurioMessage>>();
        }
   
        public bool IsLocked
        {
            get 
            { 
                return false; 
            }
        }

        public AppCryptoManagerType CryptoType
        {
	        get 
            { 
                return AppCryptoManagerType.None; 
            }
        }

        public bool Unlock(ICryptoManager cryptoManager, string passphrase)
        {
            return true;
        }

        public void Lock(ICryptoManager cryptoManager, string passphrase)
        {
 	       // Never actually locks 
        }

        public List<IMercurioMessage> GetMessages(string identifier)
        {
            if (identifier == null || identifier == string.Empty)
                throw new ArgumentException("No identifier provided to MemoryMessageStore:Retrieve()");
            if (messageStore.ContainsKey(identifier))
            {
                return new List<IMercurioMessage>(messageStore[identifier]);
            }
            else
            {
                return new List<IMercurioMessage>(); // Empty list, no messages
            }
        }

        /// <summary>
        /// Store (and potentially replace) a message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="identifier">User address</param>
        /// <returns>True if the message was replaced</returns>
        public bool Store(IMercurioMessage message, string identifier)
        {
            List<IMercurioMessage> messageList;
            bool found = false;
            if (messageStore.ContainsKey(identifier))
            {
                messageList = messageStore[identifier];
                // Replace an existing message
                for (int i = 0; i < messageList.Count; i++)
                {
                    if (messageList[i].ContentID == message.ContentID)
                    {
                        found = true;
                        messageList[i] = message;
                        break;
                    }
                }
                if (!found)
                {
                    messageList.Add(message);
                }
            }
            else
            {
                messageList = new List<IMercurioMessage>();
                messageList.Add(message);
            }

            messageStore[identifier] = messageList;
            return found;
        }  
    }
}
