using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ConnectInvitationMessage : IMercurioMessage
    {
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

        private string publicKey;
        private string[] signatures;
        private string evidence;

        public ConnectInvitationMessage(string publicKey, string[] signatures, string evidence)
        {
            this.publicKey = publicKey;
            this.signatures = signatures;
            this.evidence = evidence;
        }
    }
}
