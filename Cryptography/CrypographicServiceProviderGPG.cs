using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Mercurio.Domain;

namespace Cryptography.GPG
{
    public class CrypographicServiceProviderGPG : ICryptographicServiceProvider
    {
        public string GetProviderType()
        {
            return CryptoType.GPG.ToString();
        }

        public bool IsInstalled()
        {
            throw new NotImplementedException();
        }
    }
}
