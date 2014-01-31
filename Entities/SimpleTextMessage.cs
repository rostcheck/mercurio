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
        protected Guid contentID;
        private const string SenderAddressName = "sender_address";
        private const string RecipientAddressName = "recipient_address";
        private const string ContentName = "content";
        private const string ContentIDName = "content_id";
        protected string senderAddress;
        protected string recipientAddress;
        protected string content;

        public Guid ContentID
        {
            get
            {
                return contentID;
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

        public override string ToString()
        {
            return this.content;
        }

        public SimpleTextMessage(string senderAddress, string recipientAddress, string content)
        {
            Initialize(senderAddress, recipientAddress, content);
            this.contentID = Guid.NewGuid();
        }

        private void Initialize(string senderAddress, string recipientAddress, string content)
        {
            this.senderAddress = senderAddress;
            this.recipientAddress = recipientAddress;
            this.content = content;
        }

        public SimpleTextMessage(string senderAddress, string recipientAddress, string content, Guid contentID) 
        {
            Initialize(senderAddress, recipientAddress, content);
            this.contentID = contentID;
        }

        public SimpleTextMessage(SerializationInfo info, StreamingContext ctxt)
        {
            this.senderAddress = info.GetString(SenderAddressName);
            this.recipientAddress = info.GetString(RecipientAddressName);
            this.content = info.GetString(ContentName);
            this.contentID = (Guid)info.GetValue(ContentIDName, typeof(Guid));
        }

        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue(RecipientAddressName, recipientAddress);
            info.AddValue(SenderAddressName, senderAddress);
            info.AddValue(ContentName, content);
            info.AddValue(ContentIDName, contentID);
        }
    }
}
