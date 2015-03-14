using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Configuration entries for a CryptoManager
    /// </summary>
    public class CryptoManagerConfiguration : Dictionary<string, string>
    {
        public void Merge(Dictionary<string, string> configurationToAdd)
        {
            if (configurationToAdd != null)
            {
                foreach (var key in configurationToAdd.Keys)
                {
                    this[key] = configurationToAdd[key];
                }
            }
        }
    }
}
