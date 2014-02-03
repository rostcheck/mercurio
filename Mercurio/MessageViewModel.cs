using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Mercurio
{
    public class MessageViewModel : ViewModelBase
    {
        private object representation = null;
        private Guid messageID = Guid.Empty;

        public MessageViewModel(IMercurioMessage message)
        {
            //Note: images or other message types may require special representation
            representation = message.ToString();
        }

        public object Representation
        {
            get
            {
                return representation;
            }
            set
            {
                if (value != representation)
                {
                    representation = value;
                    RaisePropertyChangedEvent("Representation");
                }
            }
        }

        public Guid MessageID
        {
            get 
            {
                return messageID;
            }
            set
            {
                if (value != messageID)
                {
                    messageID = value;
                    RaisePropertyChangedEvent("MessageID");
                }
            }
        }
    }
}
