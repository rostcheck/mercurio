using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using Mercurio.Domain;

namespace Entities
{
    [ProtoContract]
    [Serializable]
    public class ConnectInvitationMessage : MercurioMessageBase, IMercurioMessage
    {
        private const string SenderAddressName = "sender_address";
        private const string RecipientAddressName = "recipient_address";
        private const string PublicKeyName = "public_key";
        private const string SignaturesName = "signatures";
        private const string EvidenceURLName = "evidence_url";
        private const string ContentIDName = "content_id";
        private Guid contentID;

        public Guid ContentID
        {
            get
            {
                return contentID;
            }
        }

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

        //public string[] Signatures
        //{
        //    get
        //    {
        //        return signatures;
        //    }
        //}

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

        public string KeyID { get; set; }

        private string publicKey;
        private string[] signatures;
        private string evidence;
        private string recipientAddress;
        private string senderAddress;

        public ConnectInvitationMessage(string senderAddress, string recipientAddress, string publicKey, string[] signatures, string evidence)
        {
            if (senderAddress == null || senderAddress == string.Empty)
                throw new ArgumentException("Cannot initialize ConnectInvitationMessage without senderAddress");
            if (recipientAddress == null || recipientAddress == string.Empty)
                throw new ArgumentException("Cannot initialize ConnectInvitationMessage without recipientAddress");
            if (publicKey == null || publicKey == string.Empty)
                throw new ArgumentException("Cannot initialize ConnectInvitationMessage without publicKey");
            if (evidence == null || evidence == string.Empty)
                throw new ArgumentException("Cannot initialize ConnectInvitationMessage without evidence");

            this.senderAddress = senderAddress;
            this.recipientAddress = recipientAddress;
            this.publicKey = publicKey;
            this.signatures = signatures;
            this.evidence = evidence;
            this.contentID = Guid.NewGuid();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(RecipientAddressName, recipientAddress);
            info.AddValue(SenderAddressName, senderAddress);
            info.AddValue(PublicKeyName, publicKey);
            info.AddValue(SignaturesName, signatures);
            info.AddValue(EvidenceURLName, evidence);
            info.AddValue(ContentIDName, contentID);
        }

        public ConnectInvitationMessage(SerializationInfo info, StreamingContext ctxt)
        {
            this.recipientAddress = info.GetString(RecipientAddressName);
            this.senderAddress = info.GetString(SenderAddressName);
            this.publicKey = info.GetString(PublicKeyName);
            this.signatures = (string[]) info.GetValue(SignaturesName, typeof(string[]));
            this.evidence = info.GetString(EvidenceURLName);
            this.contentID = (Guid)info.GetValue(ContentIDName, typeof(Guid));
        }

        public IMercurioMessage Process(ICryptoManager cryptoManager, Mercurio.Domain.Serializer serializer, string userIdentity)
        {
            KeyID = cryptoManager.ImportKey(PublicKey);
            RaiseMessageIsDisplayable(this);
            return null;
        }
    }
}
