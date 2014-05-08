using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            StorageCredentials storageCredentials = new StorageCredentials("mercuriotest", "phLccWS1ZhzaGX2ibfUMv9Q3kXgNhwrgz6FmwPazTZy3g7qHB1X1tEcY+pT2AJYR+7Td1Ot0zy24fIBboakoww==");
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            CloudQueue queue = queueClient.GetQueueReference("testqueue");
            queue.CreateIfNotExists();
         
            CloudQueueMessage queueMessage = new CloudQueueMessage("foolish test");
            queue.AddMessage(queueMessage);
        }
    }
}
