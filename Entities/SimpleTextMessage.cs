using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Mercurio.Domain;

namespace Entities
{
    [Serializable]
    public class SimpleTextMessage : MercurioMessageBase, IMercurioMessage
    {
        public SimpleTextMessage(string senderAddress, string recipientAddress, string content)
        {
            base.Initialize(senderAddress, recipientAddress, content);
        }

        public SimpleTextMessage(string senderAddress, string recipientAddress, string content, Guid contentID) 
        {
            base.Initialize(senderAddress, recipientAddress, content);
        }

        public SimpleTextMessage(SerializationInfo info, StreamingContext context)
        {
			base.Deserialize(info, context);
        }						
    }
}
