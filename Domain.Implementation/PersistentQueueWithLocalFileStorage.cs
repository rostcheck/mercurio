using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Mercurio.Domain;

namespace Mercurio.Domain.Implementation
{
    class PersistentQueueWithLocalFileStorage : IPersistentQueue
    {
        private Serializer serializer;
		private string name;

        public PersistentQueueWithLocalFileStorage(PersistentQueueConfiguration configuration, Serializer serializer)
        {
			if (configuration == null)
				throw new ArgumentException("Must supply configuration to PersistetnQueueWithLocalFileStorage");
			
            this.serializer = serializer;
			this.name = configuration.Name;
        }

		public string Name
		{
			get
			{
				return name;
			}
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
