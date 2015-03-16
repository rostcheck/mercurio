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
            Stream decryptedStream = cryptoManager.Decrypt(encryptedStream);
            decryptedStream.Flush();
            decryptedStream.Position = 0;
            StreamReader reader = new StreamReader(decryptedStream);
            string decrypted = reader.ReadToEnd();
            decryptedStream.Position = 0;
            return serializer.Deserialize<T>(decryptedStream);
        }

        public static T Decrypt<T>(this ICryptoManager cryptoManager, byte[] encryptedData, Serializer serializer)
        {
            MemoryStream encryptedStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(encryptedStream);
            writer.Write(encryptedData);
            writer.Flush();
            encryptedStream.Position = 0;
            return Decrypt<T>(cryptoManager, encryptedStream, serializer);
        }
    }
}
