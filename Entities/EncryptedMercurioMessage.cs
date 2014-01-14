using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;

namespace Entities
{
    [Serializable]
    public class EncryptedMercurioMessage : IMercurioMessage
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

        public EncryptedMercurioMessage(ICryptoManager cryptoManager, Serializer serializer, IMercurioMessage message)
        {
            this.senderAddress = message.SenderAddress;
            this.recipientAddress = message.RecipientAddress;

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

        public EncryptedMercurioMessage(string senderAddress, string recipientAddress, string content)
        {
            this.senderAddress = senderAddress;
            this.recipientAddress = recipientAddress;
            this.content = content;
        }

        public EncryptedMercurioMessage(SerializationInfo info, StreamingContext ctxt)
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
