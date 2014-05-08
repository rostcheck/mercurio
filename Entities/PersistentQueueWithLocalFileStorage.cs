using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Entities
{
    class PersistentQueueWithLocalFileStorage : IPersistentQueue
    {
        private Serializer serializer;

        public PersistentQueueWithLocalFileStorage(IPersistentQueueConfiguration configuration, Serializer serializer)
        {
            // Configuration isn't used for anything in this class
            this.serializer = serializer;
        }

        public void Add(EnvelopedMercurioMessage message)
        {
            Queue<EnvelopedMercurioMessage> messages = OpenQueue(message.RecipientAddress);
            messages.Enqueue(message);
            serializer.Serialize(message.RecipientAddress, messages);
        }

        public EnvelopedMercurioMessage GetNext(string address)
        {
            Queue<EnvelopedMercurioMessage> messages = OpenQueue(address);
            if (messages.Count == 0)
            {
                return null; // queue empty
            }
            else
            {
                EnvelopedMercurioMessage message = messages.Dequeue();
                serializer.Serialize(address, messages);
                return message;
            }
        }

        public int Length(string address)
        {
            Queue<EnvelopedMercurioMessage> messages = OpenQueue(address);
            return messages.Count;
        }

        private Queue<EnvelopedMercurioMessage> OpenQueue(string queueFileName)
        {
            Queue<EnvelopedMercurioMessage> messages;
            if (File.Exists(queueFileName) && new FileInfo(queueFileName).Length > 0)
                messages = serializer.Deserialize<Queue<EnvelopedMercurioMessage>>(queueFileName);
            else
                messages = new Queue<EnvelopedMercurioMessage>();
            return messages;
        }
    }
}
