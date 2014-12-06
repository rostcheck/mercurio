using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// A unique cryptographic key.
    /// </summary>
    public class Identity
    {
        public string UniqueIdentifier { get; private set; }
        public string PublicPart { get; private set; }
        public string PrivatePart { get; private set; }
    }
}
