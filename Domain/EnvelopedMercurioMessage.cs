using Mercurio.Domain;
//using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Very basic class containing addresses, payload, and type. Recipient must decide if
    /// it can deserialize this message and, if so, deserialize it. Message queues handle only
    /// EnvelopedMercurioMessage (so envelopes containing unrecognized messages can be 
    /// deserialized and discarded)
    /// </summary>
//    [ProtoContract]
    [Serializable]
    public class EnvelopedMercurioMessage : MercurioMessageBase
    {
		private string _messageType;
		private byte[] _payload;


        public string MessageType
        {
            get
            {
                return _messageType;
            }
        }

        //public IMercurioMessage Payload
        public byte[] Payload
        {
            get
            {
                //if (payload == null || payload == string.Empty)
                //{
                //    return null;
                //}
                //else
                //{
                //    MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(payload));
                //    BinaryFormatter formatter = new BinaryFormatter();
                //    //return (IMercurioMessage)formatter.Deserialize(stream);
                //    IMercurioMessage message = ProtoBuf.Serializer.Deserialize<IMercurioMessage>(stream);
                //    return message;
                //}
                return _payload;
            }

            //set
            //{
            //    SetPayload(value);
            //}
        }

        public IMercurioMessage PayloadAsMessage(Mercurio.Domain.Serializer serializer)
        {
            if (_payload == null)
            {
                return null;
            }
            else
            {
                //MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(payload));
                MemoryStream stream = new MemoryStream(_payload);
                return serializer.Deserialize<IMercurioMessage>(stream);
            }
           
        }

        private void SetPayload(IMercurioMessage message, Mercurio.Domain.Serializer serializer)
        {
            MemoryStream stream = new MemoryStream();
            //BinaryFormatter formatter = new BinaryFormatter();
            //formatter.Serialize(stream, value);
            //ProtoBuf.Serializer.Serialize(stream, value);
            serializer.Serialize(stream, message);
            stream.Flush();
            stream.Position = 0;

            //StreamReader reader = new StreamReader(stream);
            //payload = reader.ReadToEnd();
            _payload = stream.ToArray();
        }

        public EnvelopedMercurioMessage(string senderAddress, string recipientAddress, IMercurioMessage payloadMessage, Mercurio.Domain.Serializer serializer)
        {
			ValidateParameter("SenderAddress", senderAddress);
			ValidateParameter("RecipientAddress", recipientAddress);
            if (payloadMessage == null)
                throw new ArgumentException("Cannot initialize EnvelopedMercurioMessage without payloadMessage");
            if (serializer == null)
                throw new ArgumentException("Cannot initialize EnvelopedMercurioMessage without serializer");
			this._messageType = payloadMessage.GetType().ToString();
			SetPayload(payloadMessage, serializer);			
			Initialize(senderAddress, recipientAddress, GetContent(this._messageType, this._payload));
        }

		private string GetContent(string messageType, byte[] payload)
		{
			return messageType + ContentSeparator + payload.ToString();
		}

        public EnvelopedMercurioMessage(SerializationInfo info, StreamingContext context)
        {
			base.Deserialize(info, context);
			var fields = this.Content.Split(ContentSeparator.ToCharArray()[0]);
			if (fields.Length != 2)
				throw new MercurioException("EnvelopedMercurioMessage does not contain correct content");
			this._messageType = fields[0];
			this._payload = Encoding.Unicode.GetBytes(fields[1]);
        }
    }
}
