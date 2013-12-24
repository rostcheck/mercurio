using System;
using Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestEntities
{
    [TestClass]
    public class PersistentQueueTest
    {
        [TestMethod]
        public void EnqueueMessageTest()
        {
            string senderAddress = "alice@naker.net";
            string recipientAddress = "davidr@maker.net";
            string firstMessage = "first dummy message";
            string secondMessage = "second dummy message";
            IPersistentQueue queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage);
            while (queue.Length(recipientAddress) > 0) { queue.GetNext(recipientAddress); } // clear queue

            IMercurioMessage message = new DummyMessage(senderAddress, recipientAddress, firstMessage);
            queue.Add(message);
            message = new DummyMessage(senderAddress, recipientAddress, secondMessage);
            queue.Add(message);

            // Create a new queue - should read the persisted file
            queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage);
            message = queue.GetNext(recipientAddress);
            Assert.IsTrue(message.ToString() == firstMessage);
            Assert.IsTrue(message.RecipientAddress == recipientAddress);
            message = queue.GetNext(recipientAddress);
            Assert.IsTrue(message.ToString() == secondMessage);
            Assert.IsTrue(message.RecipientAddress == recipientAddress);
            Assert.IsTrue(queue.Length(recipientAddress) == 0);
        }
    }
}
