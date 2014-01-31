using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Entities
{
    public class DelayedMessage : IMercurioMessage
    {
        private int delayInMS;
        private IMercurioMessage message;

        public DelayedMessage(int delayInMS, IMercurioMessage message)
        {
            this.delayInMS = delayInMS;
            this.message = message;
        }

        public Guid ContentID
        {
            get
            {
                return message.ContentID;
            }
        }
        public string SenderAddress
        {
            get
            {
                return message.SenderAddress;
            }
        }

        public string RecipientAddress
        {
            get
            {
                return message.RecipientAddress;
            }
        }

        public bool Encryptable
        {
            get
            {
                return message.Encryptable;
            }
        }

        public string Content
        {
            get
            {
                return message.Content;
            }
        }

        public IMercurioMessage Message
        {
            get
            {
                return message;
            }
        }

        public void GetObjectData(SerializationInfo info,StreamingContext context)
        {
            message.GetObjectData(info, context);
        }
    }
}
