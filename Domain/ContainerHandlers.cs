using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    // Signatures for event handlers
    public delegate void StoreDocumentVersionHandler(Guid containerId, DocumentVersion documentVersion);
    public delegate DocumentVersion RetrieveDocumentVersionHandler(Guid containerId, DocumentVersionMetadata documentVersionMetadata);
}
