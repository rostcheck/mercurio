using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Messages
{
    public class DelayedMessageProcessor : IMercurioMessageProcessor
    {
        public string MessageTypeName
        {
            get { return "Entities.DelayedMessage"; }
        }

        public IMercurioMessage ProcessMessage(IMercurioMessage message, ICryptoManager cryptoManager, IMercurioUserAgent ui, Serializer serializer)
        {
            return ((DelayedMessage)message).Message;
        }
    }
}
