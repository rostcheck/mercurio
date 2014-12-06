using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public static class CryptoManagerFactory
    {
        private static Dictionary<string, Func<Dictionary<ConfigurationKeyEnum, string>, ICryptoManager>> registry;

        public static ICryptoManager Create(CryptoType managerType, Dictionary<ConfigurationKeyEnum, string> configuration)
        {
            if (registry == null)
            {
                throw new MercurioException("No CryptoManager types are registered with CryptoManagerFactory");
            }
            if (registry.ContainsKey(managerType.ToString()))
            {
                return registry[managerType.ToString()](configuration);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static void Register(string managerType, Func<Dictionary<ConfigurationKeyEnum, string>, ICryptoManager> construct)
        {
            if (registry == null)
            {
                registry = new Dictionary<string, Func<Dictionary<ConfigurationKeyEnum, string>, ICryptoManager>>();
            }
            if (!registry.ContainsKey(managerType.ToLower()))
            {
                registry.Add(managerType, construct);
            }
        }
    }
}
