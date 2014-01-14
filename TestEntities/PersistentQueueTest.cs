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
            Serializer serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            IPersistentQueue queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage, serializer);
            while (queue.Length(recipientAddress) > 0) { queue.GetNext(recipientAddress); } // clear queue

            IMercurioMessage message = new DummyMessage(senderAddress, recipientAddress, firstMessage);
            EnvelopedMercurioMessage envelopedMessage = new EnvelopedMercurioMessage(senderAddress, recipientAddress, message, serializer);
            queue.Add(envelopedMessage);
            message = new DummyMessage(senderAddress, recipientAddress, secondMessage);
            envelopedMessage = new EnvelopedMercurioMessage(senderAddress, recipientAddress, message, serializer);
            queue.Add(envelopedMessage);

            // Create a new queue - should read the persisted file
            queue = PersistentQueueFactory.Create(PeristentQueueType.LocalFileStorage, serializer);
            envelopedMessage = queue.GetNext(recipientAddress);
            message = envelopedMessage.PayloadAsMessage(serializer);
            Assert.IsTrue(message.ToString() == firstMessage);
            Assert.IsTrue(message.RecipientAddress == recipientAddress);
            envelopedMessage = queue.GetNext(recipientAddress);
            message = envelopedMessage.PayloadAsMessage(serializer);
            Assert.IsTrue(message.ToString() == secondMessage);
            Assert.IsTrue(message.RecipientAddress == recipientAddress);
            Assert.IsTrue(queue.Length(recipientAddress) == 0);
        }
    }
}
