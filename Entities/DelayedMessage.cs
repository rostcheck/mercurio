using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Mercurio.Domain;

namespace Entities
{
    public class DelayedMessage : MercurioMessageBase, IMercurioMessage
    {
		public int DelayInMS { get; private set; }
        private IMercurioMessage _message;

        public DelayedMessage(int delayInMS, IMercurioMessage message)
        {
            this.DelayInMS = delayInMS;
            this._message = message;
        }

        public override Guid ContentID
        {
            get
            {
                return _message.ContentID;
            }
        }

        public override string SenderAddress
        {
            get
            {
                return _message.SenderAddress;
            }
        }

        public override string RecipientAddress
        {
            get
            {
                return _message.RecipientAddress;
            }
        }

        public override bool Encryptable
        {
            get
            {
                return _message.Encryptable;
            }
        }

        public override string Content
        {
            get
            {
                return _message.Content;
            }
        }

        public IMercurioMessage Message
        {
            get
            {
                return _message;
            }
        }

        public override void GetObjectData(SerializationInfo info,StreamingContext context)
        {
            _message.GetObjectData(info, context);
        }

        public override IMercurioMessage Process(ICryptoManager cryptoManager, Serializer serializer, string userIdentity)
        {
			base.Process(cryptoManager, serializer, userIdentity);
            return Message;
        }
    }
}
