using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Entities;

namespace TestEntities
{
    [Serializable()]
    public class DummyMessage : IMercurioMessage
    {
        private string message;
        private string senderAddress;
        private string recipientAddress;
        private const string SenderAddressName = "sender_address";
        private const string RecipientAddressName = "recipient_address";
        private const string MessageName = "message";

        public string Message
        {
            get
            {
                return message;
            }
        }

        public string SenderAddress
        {
            get
            {
                return senderAddress;
            }
        }

        public string RecipientAddress
        {
            get
            {
                return recipientAddress;
            }
        }

        public bool Encryptable
        {
            get
            {
                return false;
            }
        }

        public DummyMessage(string senderAddress, string recipientAddress, string message)
        {
            this.senderAddress = senderAddress;
            this.recipientAddress = recipientAddress;
            this.message = message;
        }

        public override string ToString()
        {
            return message;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(SenderAddressName, senderAddress);
            info.AddValue(RecipientAddressName, recipientAddress);
            info.AddValue(MessageName, message);
        }

        public DummyMessage(SerializationInfo info, StreamingContext ctxt)
        {
            this.senderAddress = info.GetString(SenderAddressName);
            this.recipientAddress = info.GetString(RecipientAddressName);
            this.message = info.GetString(MessageName);
        }
    }
}
