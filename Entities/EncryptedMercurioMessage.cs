using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using Mercurio.Domain;

namespace Entities
{
    [Serializable]
    public class EncryptedMercurioMessage : MercurioMessageBase, IMercurioMessage
    {
        private const string SenderAddressName = "sender_address";
        private const string RecipientAddressName = "recipient_address";
        private const string ContentName = "content";
        private const string ContentIDName = "content_id";
        private string senderAddress;
        private string recipientAddress;
        private string content;
        private Guid contentID;


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
                return TextRepresentation().ToString();
                //return content;
            }
        }

        public bool Encryptable
        {
            get
            {
                return true;
            }
        }

        public EncryptedMercurioMessage(ICryptoManager cryptoManager, Serializer serializer, IMercurioMessage message)
        {
            if (cryptoManager == null)
                throw new ArgumentException("Cannot initialize EncryptedMercurioMessage without cryptoManager");
            if (serializer == null)
                throw new ArgumentException("Cannot initialize EncryptedMercurioMessage without serializer");
            if (message == null)
                throw new ArgumentException("Cannot initialize EncryptedMercurioMessage without message");

            this.senderAddress = message.SenderAddress;
            this.recipientAddress = message.RecipientAddress;
            this.contentID = message.ContentID;

            // Serialize the message to a Stream
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, message);
            stream.Flush();
            stream.Position = 0;

            // Encrypt it and store it as our content
            Stream encryptedStream = cryptoManager.Encrypt(stream, recipientAddress);
            encryptedStream.Position = 0;
            StreamReader reader = new StreamReader(encryptedStream);
            content = reader.ReadToEnd();
        }

        // Returns a simple text message representing the (still encrypted) message
        public SimpleTextMessage TextRepresentation()
        {
            // Show a piece of the encrypted message
            return new SimpleTextMessage(senderAddress, recipientAddress, content.Substring(71, 170));
        }

        public IMercurioMessage Decrypt(ICryptoManager cryptoManager, Serializer serializer)
        {
            if (content == null)
                throw new Exception("Message contains no content to decrypt");

            MemoryStream encryptedStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(encryptedStream);
            writer.Write(content);
            writer.Flush();
            encryptedStream.Position = 0;
            Stream decryptedStream = cryptoManager.Decrypt(encryptedStream);
            decryptedStream.Flush();
            decryptedStream.Position = 0;
            StreamReader reader = new StreamReader(decryptedStream);
            string decrypted = reader.ReadToEnd();
            decryptedStream.Position = 0;
            return serializer.Deserialize<IMercurioMessage>(decryptedStream);
        }

        public override string ToString()
        {
            //return content;
            return TextRepresentation().ToString();
        }

        public EncryptedMercurioMessage(string senderAddress, string recipientAddress, string content, Guid contentID)
        {
            this.senderAddress = senderAddress;
            this.recipientAddress = recipientAddress;
            this.content = content;
            this.contentID = contentID;
        }

        public EncryptedMercurioMessage(SerializationInfo info, StreamingContext ctxt)
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

        public IMercurioMessage Process(ICryptoManager cryptoManager, Serializer serializer, string userIdentity)
        {
            RaiseMessageIsDisplayable(this); // Listeners may display encrypted message

            IMercurioMessage decryptedMessage = Decrypt(cryptoManager, serializer);
            return new DelayedMessage(700, decryptedMessage);
        }
    }
}
