using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Scans the operating environment to find the available cryptographic providers, storage substrates, and similar resources
    /// </summary>
    public interface IEnvironmentScanner
    {
         List<ICryptographicServiceProvider> GetCryptographicProviders();
         List<IStorageSubstrate> GetStorageSubstrates();
         ITempStorageSubstrate GetTemporaryStorageSubstrate();
         string GetEditor();
         OSType GetOsType();
    }
}
