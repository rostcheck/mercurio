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
        public override string Content
        {
            get
            {
                return TextRepresentation().ToString();
                //return content;
            }
        }

        public EncryptedMercurioMessage(ICryptoManager cryptoManager, Serializer serializer, IMercurioMessage message)
        {			
			if (message == null)
                throw new ArgumentException("Cannot initialize EncryptedMercurioMessage without message");
			
            // Serialize the message to a Stream
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, message);
            stream.Flush();
            stream.Position = 0;

            // Encrypt it and store it as our content
            Stream encryptedStream = cryptoManager.Encrypt(stream, this.RecipientAddress);
            encryptedStream.Position = 0;
            StreamReader reader = new StreamReader(encryptedStream);
			this.Content = reader.ReadToEnd();
			this.Initialize(message.SenderAddress, message.RecipientAddress, message.Content, message.ContentID);
        }

        // Returns a simple text message representing the (still encrypted) message
        public SimpleTextMessage TextRepresentation()
        {
            // Show a piece of the encrypted message
            return new SimpleTextMessage(SenderAddress, RecipientAddress, Content.Substring(71, 170));
        }

        public IMercurioMessage Decrypt(ICryptoManager cryptoManager, Serializer serializer)
        {
            if (this.Content == null)
                throw new Exception("Message contains no content to decrypt");

            MemoryStream encryptedStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(encryptedStream);
            writer.Write(this.Content);
            writer.Flush();
            encryptedStream.Position = 0;
            //Stream decryptedStream = cryptoManager.Decrypt(encryptedStream);
            //decryptedStream.Flush();
            //decryptedStream.Position = 0;
            //StreamReader reader = new StreamReader(decryptedStream);
            //string decrypted = reader.ReadToEnd();
            //decryptedStream.Position = 0;
            //return serializer.Deserialize<IMercurioMessage>(decryptedStream);
            return cryptoManager.Decrypt<IMercurioMessage>(encryptedStream, serializer);
        }

        public override string ToString()
        {
            //return content;
            return TextRepresentation().ToString();
        }

        public EncryptedMercurioMessage(string senderAddress, string recipientAddress, string content, Guid contentID)
		{
			this.Initialize(senderAddress, recipientAddress, content, contentID);
        }

        public EncryptedMercurioMessage(SerializationInfo info, StreamingContext context)
        {
			base.Deserialize(info, context);
        }			

        public override IMercurioMessage Process(ICryptoManager cryptoManager, Serializer serializer, string userIdentity)
        {
            IMercurioMessage decryptedMessage = Decrypt(cryptoManager, serializer);
			RaiseMessageIsDisplayable(this); // Listeners may display encrypted message
            return new DelayedMessage(700, decryptedMessage);
        }
    }
}
