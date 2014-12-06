using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public static class CryptographicServiceProviderFactory
    {
        public static ICryptographicServiceProvider Create(CryptoType cryptoType)
        {
            switch(cryptoType)
            {
                case CryptoType.GPG:
                default:
                    throw new NotImplementedException();
                   // return new CryptographicServiceProviderGPG();
            }
        }

        //public static Register(CryptoType cryptoType, )
    }
}
