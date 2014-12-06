using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Cryptography.GPG
{
    public class CrypographicServiceProviderGPG : ICryptographicServiceProvider
    {
        public CryptoType GetProviderType()
        {
            return CryptoType.GPG;
        }

        public bool IsInstalled()
        {
            throw new NotImplementedException();
        }
    }
}
