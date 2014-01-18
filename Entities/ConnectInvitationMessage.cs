using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Entities
{
    [ProtoContract]
    [Serializable]
    public class ConnectInvitationMessage : IMercurioMessage
    {
        private const string SenderAddressName = "sender_address";
        private const string RecipientAddressName = "recipient_address";
        private const string PublicKeyName = "public_key";
        private const string SignaturesName = "signatures";
        private const string EvidenceURLName = "evidence_url";

        [ProtoMember(1)]
        public string PublicKey
        {
            get
            {
                return publicKey;
            }

            set
            {
                publicKey = value;
            }
        }

        public string Content
        {
            get
            {
                return publicKey;
            }
        }

        //[ProtoMember(2)]
        //public string[] Signatures
        //{
        //    get
        //    {
        //        return signatures;
        //    }
        //}

        [ProtoMember(3)]
        public string Evidence
        {
            get
            {
                return evidence;
            }

            set
            {
                evidence = value;
            }
        }

        [ProtoMember(4)]
        public string RecipientAddress
        {
            get
            {
                return recipientAddress;
            }

            set
            {
                recipientAddress = value;
            }
        }

        [ProtoMember(5)]
        public string SenderAddress
        {
            get
            {
                return senderAddress;
            }

            set
            {
                senderAddress = value;
            }
        }

        [ProtoMember(6)]
        public bool Encryptable
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        private string publicKey;
        private string[] signatures;
        private string evidence;
        private string recipientAddress;
        private string senderAddress;

        public ConnectInvitationMessage(string senderAddress, string recipientAddress, string publicKey, string[] signatures, string evidence)
        {
            this.senderAddress = senderAddress;
            this.recipientAddress = recipientAddress;
            this.publicKey = publicKey;
            this.signatures = signatures;
            this.evidence = evidence;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(RecipientAddressName, recipientAddress);
            info.AddValue(SenderAddressName, senderAddress);
            info.AddValue(PublicKeyName, publicKey);
            info.AddValue(SignaturesName, signatures);
            info.AddValue(EvidenceURLName, evidence);
        }

        public ConnectInvitationMessage(SerializationInfo info, StreamingContext ctxt)
        {
            this.recipientAddress = info.GetString(RecipientAddressName);
            this.senderAddress = info.GetString(SenderAddressName);
            this.publicKey = info.GetString(PublicKeyName);
            this.signatures = (string[]) info.GetValue(SignaturesName, typeof(string[]));
            this.evidence = info.GetString(EvidenceURLName);
        }
    }
}
