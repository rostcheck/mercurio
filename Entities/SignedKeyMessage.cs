using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Entities
{
    [Serializable]
    public class SignedKeyMessage : IMercurioMessage
    {
        private Guid contentID;
        private const string SenderAddressName = "sender_address";
        private const string RecipientAddressName = "recipient_address";
        private const string SignedPublicKeyName = "signed_public_key";
        private const string EvidenceURLName = "evidence_url";
        private const string ContentIDName = "content_id";
        private string senderAddress;
        private string recipientAddress;
        private string signedPublicKey;
        private string evidence;

        public Guid ContentID
        {
            get
            {
                return contentID;
            }
        }

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

        public string SignedPublicKey
        {
            get
            {
                return signedPublicKey;
            }
        }

        public string Content
        {
            get
            {
                return signedPublicKey;
            }
        }

        public bool Encryptable
        {
            get
            {
                return false;
            }
        }

        public SignedKeyMessage(string recipientAddress, string senderAddress, string signedPublicKey, string evidence)
        {
            if (senderAddress == null || senderAddress == string.Empty)
                throw new ArgumentException("Cannot initialize SignedKeyMessage without senderAddress");
            if (recipientAddress == null || recipientAddress == string.Empty)
                throw new ArgumentException("Cannot initialize SignedKeyMessage without recipientAddress");
            if (signedPublicKey == null || signedPublicKey == string.Empty)
                throw new ArgumentException("Cannot initialize SignedKeyMessage without signedPublicKey");

            this.senderAddress = senderAddress;
            this.recipientAddress = recipientAddress;
            this.signedPublicKey = signedPublicKey;
            this.evidence = evidence;
            this.contentID = Guid.NewGuid();
        }

        public SignedKeyMessage(SerializationInfo info, StreamingContext ctxt)
        {
            this.senderAddress = info.GetString(SenderAddressName);
            this.recipientAddress = info.GetString(RecipientAddressName);
            this.signedPublicKey = info.GetString(SignedPublicKeyName);
            this.evidence = info.GetString(EvidenceURLName);
            this.contentID = (Guid)info.GetValue(ContentIDName, typeof(Guid));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(RecipientAddressName, recipientAddress);
            info.AddValue(SenderAddressName, senderAddress);
            info.AddValue(SignedPublicKeyName, SignedPublicKey);
            info.AddValue(EvidenceURLName, evidence);
            info.AddValue(ContentIDName, contentID);
        }
    }
}
