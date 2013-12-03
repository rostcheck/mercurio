using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public static class CryptoManagerFactory
    {
        public static ICryptoManager Create(CryptoManagerType managerType, Dictionary<ConfigurationKeyEnum, string> configuration)
        {
            switch (managerType)
            {
                case CryptoManagerType.GPGCryptoManager:
                    return new GPGManager(configuration);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
