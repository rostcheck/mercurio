using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Entities
{
    public class PersistentQueueWithCloudStorage : IPersistentQueue
    {
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;
        private Serializer serializer;

        public PersistentQueueWithCloudStorage(IPersistentQueueConfiguration configuration, Serializer serializer)
        {
            storageAccount = CloudStorageAccount.Parse(configuration.ConfigurationString);
            queueClient = storageAccount.CreateCloudQueueClient();
            this.serializer = serializer;
        }

        public void Add(EnvelopedMercurioMessage message)
        {
            CloudQueue queue = queueClient.GetQueueReference(MakeQueueName(message.RecipientAddress));
            queue.CreateIfNotExists();
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, message);            
            CloudQueueMessage queueMessage = new CloudQueueMessage(stream.ToArray());
            queue.AddMessage(queueMessage);
        }

        public EnvelopedMercurioMessage GetNext(string address)
        {
            CloudQueue queue = queueClient.GetQueueReference(MakeQueueName(address));
            queue.CreateIfNotExists();
            CloudQueueMessage retrievedMessage = queue.GetMessage();
            if (retrievedMessage != null)
            {
                MemoryStream stream = new MemoryStream(retrievedMessage.AsBytes);
                EnvelopedMercurioMessage returnMessage = serializer.Deserialize<EnvelopedMercurioMessage>(stream);
                queue.DeleteMessage(retrievedMessage);
                return returnMessage;
            }
            else return null;
        }

        public int Length(string address)
        {
            CloudQueue queue = queueClient.GetQueueReference(MakeQueueName(address));
            queue.CreateIfNotExists();
            return queue.ApproximateMessageCount.HasValue ? queue.ApproximateMessageCount.Value : 0;
        }

        private static string MakeQueueName(string address)
        {
            string possibleName = address.ToLower();
            possibleName = possibleName.Replace("@", "-at-");
            possibleName = possibleName.Replace(".", "-dot-");
            ValidateQueueName(possibleName);
            return possibleName;
        }

        private static void ValidateQueueName(String name)
        {
            if (String.IsNullOrEmpty(name))
                throw new InvalidQueueNameException("Queue name can't be null or empty");

            if (name.Length < 3 || name.Length > 63)
                throw new InvalidQueueNameException("Queue name must be from 3 to 63 characters long ");

            if (name[0] == '-' || name[name.Length - 1] == '-')
                throw new InvalidQueueNameException("Queue name may not begin or end with dash character (-)");

            foreach (Char ch in name)
            {
                if (Char.IsUpper(ch))                
                    throw new InvalidQueueNameException("Queue names must be all lower case");
                
                if (!Char.IsLetterOrDigit(ch) && ch != '-')
                    throw new InvalidQueueNameException("Queue name can contain only letters, numbers, and and dash characters (-)");
            }
        }
    }
}
