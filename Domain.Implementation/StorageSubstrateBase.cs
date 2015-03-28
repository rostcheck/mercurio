using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    public class StorageSubstrateBase
    {
        protected ContainerMetadata LoadMetadata(Stream stream, Serializer serializer)
        {
            return serializer.Deserialize<ContainerMetadata>(stream);
        }

        //protected ContainerPrivateMetadata LoadPrivateMetadata(Stream stream, Serializer serializer, ICryptoManager cryptoManager)
        //{
        //    return serializer.Deserialize<ContainerPrivateMetadata>(cryptoManager.Decrypt(stream));
        //}


    }
}
