using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Serializable]
    public class ConnectInvitationMessage : IMercurioMessage
    {
        private const string AddressName = "address";
        private const string PublicKeyName = "public_key";
        private const string SignaturesName = "signatures";
        private const string EvidenceURLName = "evidence_url";

        public string PublicKey
        {
            get
            {
                return publicKey;
            }
        }

        public string[] Signatures
        {
            get
            {
                return signatures;
            }
        }

        public string Evidence
        {
            get
            {
                return evidence;
            }
        }

        public string Address
        {
            get
            {
                return address;
            }
        }

        private string publicKey;
        private string[] signatures;
        private string evidence;
        private string address;

        public ConnectInvitationMessage(string address, string publicKey, string[] signatures, string evidence)
        {
            this.address = address;
            this.publicKey = publicKey;
            this.signatures = signatures;
            this.evidence = evidence;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(AddressName, address);
            info.AddValue(PublicKeyName, publicKey);
            info.AddValue(SignaturesName, signatures);
            info.AddValue(EvidenceURLName, Evidence);
        }

        public ConnectInvitationMessage(SerializationInfo info, StreamingContext ctxt)
        {
            this.address = info.GetString(AddressName);
            this.publicKey = info.GetString(PublicKeyName);
            this.signatures = (string[]) info.GetValue(SignaturesName, typeof(string[]));
            this.evidence = info.GetString(EvidenceURLName);
        }
    }
}
