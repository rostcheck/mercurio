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
            string address = "davidr@maker.net";
            string firstMessage = "first dummy message";
            string secondMessage = "second dummy message";
            IPersistentQueue queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage);
            while (queue.Length() > 0) { queue.GetNext(); } // clear queue

            IMercurioMessage message = new DummyMessage(address, firstMessage);
            queue.Add(message);
            message = new DummyMessage(address, secondMessage);
            queue.Add(message);

            // Create a new queue - should read the persisted file
            queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage);
            message = queue.GetNext();
            Assert.IsTrue(message.ToString() == firstMessage);
            Assert.IsTrue(message.Address == address);
            message = queue.GetNext();
            Assert.IsTrue(message.ToString() == secondMessage);
            Assert.IsTrue(message.Address == address);
            Assert.IsTrue(queue.Length() == 0);
        }
    }
}
