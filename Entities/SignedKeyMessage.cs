using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Mercurio.Domain;

namespace Entities
{
    [Serializable]
    public class SignedKeyMessage : MercurioMessageBase, IMercurioMessage
    {
        private string _signedPublicKey;
        private string _evidence;

        public string SignedPublicKey
        {
            get
            {
                return _signedPublicKey;
            }
        }

		public string Evidence
		{
			get
			{
				return _evidence;
			}
		}

       	public override string ToString()
		{
            return _signedPublicKey;            
        }

        public override bool Encryptable
        {
            get
            {
                return false; // Can't encryot the connection conversation
            }
        }

        public SignedKeyMessage(string recipientAddress, string senderAddress, string signedPublicKey, string evidence)
        {
			ValidateParameter("SignedPublicKey", signedPublicKey);
			base.Initialize(recipientAddress, senderAddress, GetContent(signedPublicKey, evidence));

            this._signedPublicKey = signedPublicKey;
            this._evidence = evidence;
        }
			
		private string GetContent(string signedPublicKey, string evidence)
		{
			return signedPublicKey + ContentSeparator + evidence;
		}

        public SignedKeyMessage(SerializationInfo info, StreamingContext context)
        {
			base.Deserialize(info, context);
			var fields = this.Content.Split(ContentSeparator.ToCharArray()[0]);
			if (fields.Length < 1)
				throw new MercurioException("SignedKeyMessage does not contain correct content");
			this._signedPublicKey = fields[0];
			if (fields.Length > 1)
				this._evidence = fields[1];
        }

        public override IMercurioMessage Process(ICryptoManager cryptoManager, Serializer serializer, string userIdentity)
        {
            //string keyID = cryptoManager.ImportKey(SignedPublicKey);
            //string fingerprint = cryptoManager.GetFingerprint(keyID);

            //TODO: further secure this protocol; insure message is expected
            return null;
        }
    }
}
