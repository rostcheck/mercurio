using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    public class DiskContainerMetadata
    {
        public string KeyFingerprint { get; private set; }
        public long PrivateDataStart { get; private set; }
        public long PrivateDataLength { get; private set; }
    }
}
