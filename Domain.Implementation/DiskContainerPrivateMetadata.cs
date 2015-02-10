using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    /// <summary>
    /// Metadata about the Container that is encrypted when stored onto the StorageSubstrate
    /// </summary>
    public class DiskContainerPrivateMetadata
    {
        public string ContainerName { get; private set; }
        public string ContainerDescription { get; private set; }
    }
}
