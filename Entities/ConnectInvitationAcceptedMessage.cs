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
    public class ConnectInvitationAcceptedMessage : IMercurioMessage
    {
        private const string SenderAddressName = "sender_address";
        private const string RecipientAddressName = "recipient_address";
        private const string SignedPublicKeyName = "signed_public_key";
        private const string SenderKeyIDName = "sender_key_id";

        public ConnectInvitationAcceptedMessage(string senderAddress, string recipientAddress, string senderKeyID, string signedPublicKey)
        {
            this.senderAddress = senderAddress;
            this.recipientAddress = recipientAddress;
            this.senderKeyID = senderKeyID;
            this.signedPublicKey = signedPublicKey;
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

        private string signedPublicKey;
        private string senderKeyID;
        private string senderAddress;
        private string recipientAddress;

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
        }

        public ConnectInvitationAcceptedMessage(SerializationInfo info, StreamingContext context)
        {
            this.recipientAddress = info.GetString(RecipientAddressName);
            this.senderAddress = info.GetString(SenderAddressName);
            this.senderKeyID = info.GetString(SenderKeyIDName);
            this.signedPublicKey = info.GetString(SignedPublicKeyName);
        }
    }
}
