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
		private	 bool _isDefaultStorageSubstrate;

		public StorageSubstrateBase(bool isDefault)
		{
			_isDefaultStorageSubstrate = isDefault;
		}

        protected ContainerMetadata LoadMetadata(Stream stream, Serializer serializer)
        {
            return serializer.Deserialize<ContainerMetadata>(stream);
        }

		// Environment has one default substrate, where the keychain is stored
		public bool IsDefaultStorageSubstrate 
		{
			get
			{
				return _isDefaultStorageSubstrate;
			}
		}
        //protected ContainerPrivateMetadata LoadPrivateMetadata(Stream stream, Serializer serializer, ICryptoManager cryptoManager)
        //{
        //    return serializer.Deserialize<ContainerPrivateMetadata>(cryptoManager.Decrypt(stream));
        //}

    }
}
