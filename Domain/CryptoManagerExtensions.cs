using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    public static class CryptoManagerExtensions
    {
        public static T Decrypt<T>(this ICryptoManager cryptoManager, Stream encryptedStream, Serializer serializer)
        {
            encryptedStream.Position = 0;
            return serializer.Deserialize<T>(cryptoManager.Decrypt(encryptedStream));
        }

        public static T Decrypt<T>(this ICryptoManager cryptoManager, byte[] encryptedData, Serializer serializer)
        {
            MemoryStream encryptedStream = new MemoryStream(encryptedData);
            encryptedStream.Position = 0;
            return Decrypt<T>(cryptoManager, encryptedStream, serializer);
        }
    }
}
