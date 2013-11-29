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
        private Queue<IMercurioMessage> messages;
        private const string queueFileName = "messages.bin";

        public PersistentQueueWithLocalFileStorage()
        {
            if (File.Exists(queueFileName))
                messages = Serializer.DeSerialize<Queue<IMercurioMessage>>(queueFileName);
            else
                messages = new Queue<IMercurioMessage>();
        }

        public void Add(IMercurioMessage message)
        {
            messages.Enqueue(message);
            Serializer.Serialize(queueFileName, messages);
        }

        public IMercurioMessage GetNext()
        {
            IMercurioMessage message = messages.Dequeue();
            Serializer.Serialize(queueFileName, messages);
            return message;
        }

        public int Length()
        {
            return messages.Count;
        }
    }
}
