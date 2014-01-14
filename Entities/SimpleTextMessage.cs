using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Entities
{
    [Serializable]
    public class SimpleTextMessage : IMercurioMessage
    {
        private const string SenderAddressName = "sender_address";
        private const string RecipientAddressName = "recipient_address";
        private const string ContentName = "content";
        private string senderAddress;
        private string recipientAddress;
        private string content;

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

        public string Content
        {
            get
            {
                return content;
            }
        }

        public bool Encryptable
        {
            get
            {
                return true;
            }
        }

        public SimpleTextMessage(string senderAddress, string recipientAddress, string content)
        {
            this.senderAddress = senderAddress;
            this.recipientAddress = recipientAddress;
            this.content = content;
        }

        public SimpleTextMessage(SerializationInfo info, StreamingContext ctxt)
        {
            this.senderAddress = info.GetString(SenderAddressName);
            this.recipientAddress = info.GetString(RecipientAddressName);
            this.content = info.GetString(ContentName);
        }

        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue(RecipientAddressName, recipientAddress);
            info.AddValue(SenderAddressName, senderAddress);
            info.AddValue(ContentName, content);
        }
    }
}
