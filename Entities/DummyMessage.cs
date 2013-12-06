using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Entities
{
    [Serializable()]
    public class DummyMessage : IMercurioMessage
    {
        private string message;
        private string address;
        private const string AddressName = "address";
        private const string MessageName = "message";

        public string Message
        {
            get
            {
                return message;
            }
        }

        public string Address
        {
            get
            {
                return address;
            }
        }

        public DummyMessage(string address, string message)
        {
            this.address = address;
            this.message = message;
        }

        public override string ToString()
        {
            return message;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(AddressName, address);
            info.AddValue(MessageName, message);
        }

        public DummyMessage(SerializationInfo info, StreamingContext ctxt)
        {
            this.address = info.GetString(AddressName);
            this.message = info.GetString(MessageName);
        }
    }
}
