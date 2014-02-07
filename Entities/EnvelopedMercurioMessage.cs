using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Entities
{
    /// <summary>
    /// Very basic class containing addresses, payload, and type. Recipient must decide if
    /// it can deserialize this message and, if so, deserialize it. Message queues handle only
    /// EnvelopedMercurioMessage (so envelopes containing unrecognized messages can be 
    /// deserialized and discarded)
    /// </summary>
    [ProtoContract]
    [Serializable]
    public class EnvelopedMercurioMessage
    {
        private const string SenderAddressName = "sender_address";
        private const string RecipientAddressName = "recipient_address";
        private const string MessageTypeName = "message_type";
        private const string PayloadName = "payload";

        public string SenderAddress 
        {
            get
            {
                return senderAddress;
            }
        }

        public string RecipientAddress
        {
            get
            {
                return recipientAddress;
            }
        }

        public string MessageType
        {
            get
            {
                return messageType;
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
                return payload;
            }

            //set
            //{
            //    SetPayload(value);
            //}
        }

        public IMercurioMessage PayloadAsMessage(Serializer serializer)
        {
            if (payload == null)
            {
                return null;
            }
            else
            {
                //MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(payload));
                MemoryStream stream = new MemoryStream(payload);
                return serializer.Deserialize<IMercurioMessage>(stream);
            }
           
        }

        private string senderAddress;
        private string recipientAddress;
        private string messageType;
        private byte[] payload;

        private void SetPayload(IMercurioMessage message, Serializer serializer)
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
            payload = stream.ToArray();
        }

        public EnvelopedMercurioMessage(string senderAddress, string recipientAddress, IMercurioMessage payload, Serializer serializer)
        {
            if (senderAddress == null || senderAddress == string.Empty)
                throw new ArgumentException("Cannot initialize EnvelopedMercurioMessage without senderAddress");
            if (recipientAddress == null || recipientAddress == string.Empty)
                throw new ArgumentException("Cannot initialize EnvelopedMercurioMessage without recipientAddress");
            if (payload == null)
                throw new ArgumentException("Cannot initialize EnvelopedMercurioMessage without payload");
            if (serializer == null)
                throw new ArgumentException("Cannot initialize EnvelopedMercurioMessage without serializer");

            this.senderAddress = senderAddress;
            this.recipientAddress = recipientAddress;
            this.messageType = payload.GetType().ToString();
            SetPayload(payload, serializer);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(SenderAddressName, senderAddress);
            info.AddValue(RecipientAddressName, recipientAddress);
            info.AddValue(MessageTypeName, messageType);
            info.AddValue(PayloadName, payload);
        }

        public EnvelopedMercurioMessage(SerializationInfo info, StreamingContext ctxt)
        {
            this.senderAddress = info.GetString(SenderAddressName);
            this.recipientAddress = info.GetString(RecipientAddressName);
            this.messageType = info.GetString(MessageTypeName);
            this.payload = Encoding.Unicode.GetBytes(info.GetString(PayloadName));
        }
    }
}
