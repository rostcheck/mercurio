using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Messages
{
    public class EncryptedMessageProcessor : IMercurioMessageProcessor
    {
        public string MessageTypeName
        {
            get { return "Entities.EncryptedMercurioMessage"; }
        }

        public IMercurioMessage ProcessMessage(IMercurioMessage message, ICryptoManager cryptoManager, IMercurioUserAgent ui, Serializer serializer)
        {
            ui.DisplayMessage(message, message.SenderAddress); // Display encrypted
            
            IMercurioMessage decryptedMessage = ((EncryptedMercurioMessage)message).Decrypt(cryptoManager, serializer);
            return new DelayedMessage(700, decryptedMessage);
        }
    }
}
