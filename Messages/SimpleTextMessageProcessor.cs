using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Messages
{
    public class SimpleTextMessageProcessor : IMercurioMessageProcessor
    {

        public string MessageTypeName
        {
            get
            {
                return "Entities.SimpleTextMessageProcessor";
            }
        }

        public IMercurioMessage ProcessMessage(IMercurioMessage message, ICryptoManager cryptoManager, IMercurioUI ui)
        {
            if (message.GetType() != typeof(SimpleTextMessage))
            {
                string messageTypeName = message.GetType().ToString();
                throw new Exception("Unknown message type " + message + " passed to SimpleTextMessageProcessor:ProcessMessage");
            }
            SimpleTextMessage thisMessage = message as SimpleTextMessage;
            ui.DisplayTextMessage(thisMessage.Content);
            return null;
        }
    }
}
