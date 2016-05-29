using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Represents an accepted invitation. Carries a signed public key from
    /// the inviter (recipient) being returned to it by to the invitee (sender)
    /// </summary>
    [Serializable]
    public class ConnectInvitationAcceptedMessage : MercurioMessageBase, IMercurioMessage
    {
        private const string SenderAddressName = "sender_address";
        private const string RecipientAddressName = "recipient_address";
        private const string SignedPublicKeyName = "signed_public_key";
        private const string SenderKeyIDName = "sender_key_id";
        private const string ContentIDName = "content_id";
        private string signedPublicKey;
        private string senderKeyID;
        private string senderAddress;
        private string recipientAddress;
        private Guid contentID;

        public Guid ContentID
        {
            get
            {
                return contentID;
            }
        }

        public ConnectInvitationAcceptedMessage(string senderAddress, string recipientAddress, string senderKeyID, string signedPublicKey)
        {
            if (senderAddress == null || senderAddress == string.Empty)
                throw new ArgumentException("Cannot initialize ConnectInvitationAcceptedMessage without senderAddress");
            if (recipientAddress == null || recipientAddress == string.Empty)
                throw new ArgumentException("Cannot initialize ConnectInvitationAcceptedMessage without recipientAddress");
            if (senderKeyID == null || senderKeyID == string.Empty)
                throw new ArgumentException("Cannot initialize ConnectInvitationAcceptedMessage without senderKeyID");
            if (signedPublicKey == null || signedPublicKey == string.Empty)
                throw new ArgumentException("Cannot initialize ConnectInvitationAcceptedMessage without signedPublicKey");

            this.senderAddress = senderAddress;
            this.recipientAddress = recipientAddress;
            this.senderKeyID = senderKeyID;
            this.signedPublicKey = signedPublicKey;
            this.contentID = Guid.NewGuid();
        }

        public string SenderKeyID
        {
            get
            {
                return senderKeyID;
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(RecipientAddressName, recipientAddress);
            info.AddValue(SenderAddressName, senderAddress);
            info.AddValue(SenderKeyIDName, senderKeyID);
            info.AddValue(SignedPublicKeyName, signedPublicKey);
            info.AddValue(ContentIDName, contentID);
        }

        public ConnectInvitationAcceptedMessage(SerializationInfo info, StreamingContext context)
        {
            this.recipientAddress = info.GetString(RecipientAddressName);
            this.senderAddress = info.GetString(SenderAddressName);
            this.senderKeyID = info.GetString(SenderKeyIDName);
            this.signedPublicKey = info.GetString(SignedPublicKeyName);
            this.contentID = (Guid)info.GetValue(ContentIDName, typeof(Guid));
        }

        public IMercurioMessage Process(ICryptoManager cryptoManager, Serializer serializer, string userIdentity)
        {
            // TODO: Is this message expected? Secure protocol more

            string keyID = cryptoManager.ImportKey(SignedPublicKey);
            //string fingerprint = cryptoManager.GetFingerprint(keyID);

            // Countersign the key and send it back
            cryptoManager.SignKey(keyID);
            //string signedKey = cryptoManager.GetPublicKey(keyID);
            // Reverse sender and recipient
            string sender = RecipientAddress;
            string recipient = SenderAddress;
            var signedKeyMessage = new SignedKeyMessage(recipient, sender, 
                cryptoManager.GetPublicKey(userIdentity), string.Empty);
            return signedKeyMessage;
        }
    }
}
