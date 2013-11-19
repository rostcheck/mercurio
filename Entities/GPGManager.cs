using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class GPGManager : ICryptoManager
    {
        public string Encrypt(string message, EncryptionAlgorithmEnum algorithm)
        {
            throw new NotImplementedException();
        }

        public string Sign(string message)
        {
            throw new NotImplementedException();
        }

        public bool Validate(string message)
        {
            throw new NotImplementedException();
        }

        public string GetPublicKey(string identity)
        {
            throw new NotImplementedException();
        }

        public string[] GetSignatures()
        {
            throw new NotImplementedException();
        }
    }
}
