using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Mercurio.Domain;

namespace Entities
{
    [Serializable()]
    public class DummyMessage : MercurioMessageBase, IMercurioMessage
    {
        private string message;
        private string senderAddress;
        private string recipientAddress;
        private const string SenderAddressName = "sender_address";
        private const string RecipientAddressName = "recipient_address";
        private const string MessageName = "message";
        private const string ContentIDName = "content_id";
        private Guid contentID;

        public Guid ContentID
        {
            get
            {
                return contentID;
            }
        }

        public string Content
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
            if (senderAddress == null || senderAddress == string.Empty)
                throw new ArgumentException("Cannot initialize DummyMessage without senderAddress");
            if (recipientAddress == null || recipientAddress == string.Empty)
                throw new ArgumentException("Cannot initialize DummyMessage without recipientAddress");

            this.senderAddress = senderAddress;
            this.recipientAddress = recipientAddress;
            this.message = message;
            this.contentID = Guid.NewGuid();
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
            info.AddValue(ContentIDName, contentID);
        }

        public DummyMessage(SerializationInfo info, StreamingContext ctxt)
        {
            this.senderAddress = info.GetString(SenderAddressName);
            this.recipientAddress = info.GetString(RecipientAddressName);
            this.message = info.GetString(MessageName);
            this.contentID = (Guid)info.GetValue(ContentIDName, typeof(Guid));
        }

        public IMercurioMessage Process(ICryptoManager cryptoManager, Serializer serializer, string userIdentity)
        {
            RaiseMessageIsDisplayable(this);
            return null;
        }
    }
}
