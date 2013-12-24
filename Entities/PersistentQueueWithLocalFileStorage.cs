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
        public PersistentQueueWithLocalFileStorage()
        {
        }

        public void Add(IMercurioMessage message)
        {
            Queue<IMercurioMessage> messages = OpenQueue(message.RecipientAddress);
            messages.Enqueue(message);
            Serializer.Serialize(message.RecipientAddress, messages);
        }

        public IMercurioMessage GetNext(string address)
        {
            Queue<IMercurioMessage> messages = OpenQueue(address);
            IMercurioMessage message = messages.Dequeue();
            Serializer.Serialize(address, messages);
            return message;
        }

        public int Length(string address)
        {
            Queue<IMercurioMessage> messages = OpenQueue(address);
            return messages.Count;
        }

        private Queue<IMercurioMessage> OpenQueue(string queueFileName)
        {
            Queue<IMercurioMessage> messages;
            if (File.Exists(queueFileName))
                messages = Serializer.DeSerialize<Queue<IMercurioMessage>>(queueFileName);
            else
                messages = new Queue<IMercurioMessage>();
            return messages;
        }
    }
}
