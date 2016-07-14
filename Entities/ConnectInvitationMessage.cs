using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
//using ProtoBuf;
using Mercurio.Domain;

namespace Entities
{
    //[ProtoContract]
    [Serializable]
    public class ConnectInvitationMessage : MercurioMessageBase, IMercurioMessage
    {
        public string PublicKey
        {
            get
            {
                return _publicKey;
            }

//            set
//            {
//                _publicKey = value;
//            }
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
                return _evidence;
            }

//            set
//            {
//                _evidence = value;
//            }
        }						

        public string KeyID { get; set; } // Transient data

        private string _publicKey;
        private string[] _signatures;
        private string _evidence;

        public ConnectInvitationMessage(string senderAddress, string recipientAddress, string publicKey, string[] signatures, string evidence)
        {
			ValidateParameter("PublicKey", publicKey);
			ValidateParameter("Evidence", evidence);
			base.Initialize(senderAddress, recipientAddress, GetContent(publicKey, signatures, evidence));

            this._publicKey = publicKey;
            this._signatures = signatures;
            this._evidence = evidence;
        }

		private string GetContent(string publicKey, string[] signatures, string evidence)
		{
			var signaturesAsString = string.Join(ContentSubSeparator, signatures);
			return publicKey + ContentSeparator + signaturesAsString + ContentSeparator + evidence;
		}			

        public ConnectInvitationMessage(SerializationInfo info, StreamingContext context)
        {
			base.Deserialize(info, context);
			var fields = this.Content.Split(ContentSeparator.ToCharArray()[0]);
			if (fields.Length != 3)
				throw new MercurioException("ConnectInvitationMessage does not contain the correct content");
			
			this._publicKey = fields[0];
			this._signatures = fields[1].Split(ContentSubSeparator.ToCharArray()[0]);
			this._evidence = fields[2];
        }

        public override IMercurioMessage Process(ICryptoManager cryptoManager, Mercurio.Domain.Serializer serializer, string userIdentity)
        {
            KeyID = cryptoManager.ImportKey(_publicKey);
			return base.Process(cryptoManager, serializer, userIdentity);
        }
    }
}
