using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// A type of cryptographic software that, if installed, can provide cryptographic services (represented via an ICryptoManager)
    /// </summary>
    public interface ICryptographicServiceProvider
    {
        string GetProviderType();
        bool IsInstalled();
        CryptoManagerConfiguration GetConfiguration();
        ICryptoManager CreateManager(CryptoManagerConfiguration configuration);
    }
}
