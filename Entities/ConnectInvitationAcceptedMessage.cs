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
        private string _signedPublicKey;
        private string _senderKeyID;

        public ConnectInvitationAcceptedMessage(string senderAddress, string recipientAddress, string senderKeyID, string signedPublicKey)
        {
			ValidateParameter("SenderKeyId", senderKeyID);
			ValidateParameter("SignedPublicKey", signedPublicKey);		
			base.Initialize(senderAddress, recipientAddress, GetContent(senderKeyID, signedPublicKey));

            this._senderKeyID = senderKeyID;
            this._signedPublicKey = signedPublicKey;
        }

		private string GetContent(string senderKeyId, string signedPublicKey)
		{
			return senderKeyId + ContentSeparator + signedPublicKey;
		}

        public string SenderKeyID
        {
            get
            {
                return _senderKeyID;
            }
        }

        public string SignedPublicKey
        {
            get
            {
                return _signedPublicKey;
            }
        }			

        public override bool Encryptable
        {
            get
            {
                return false; // Ye can't encrypt the ConnectInvitationMessage, laddie, or your new potential connection can't read it
            }
        }

        public ConnectInvitationAcceptedMessage(SerializationInfo info, StreamingContext context)
        {
			Deserialize(info, context);
			var fields = this.Content.Split(ContentSeparator.ToCharArray()[0]);
			if (fields.Length != 2)
				throw new MercurioException("ConnectInvitationMessage does not contain correct content");
			this._senderKeyID = fields[0];
			this._signedPublicKey = fields[1];

        }

        public override IMercurioMessage Process(ICryptoManager cryptoManager, Serializer serializer, string userIdentity)
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
			base.Process(cryptoManager, serializer, userIdentity);
            return signedKeyMessage;
        }
    }
}
