using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Mercurio.Domain
{
    public class BinarySerializerCore : ISerializerCore
    {
        private BinaryFormatter bFormatter;

        public BinarySerializerCore()
        {
            bFormatter = new BinaryFormatter();

        }
        public void Serialize(Stream stream, object objectToSerialize)
        {
            bFormatter.Serialize(stream, objectToSerialize);
        }

        public T Deserialize<T>(Stream stream)
        {
            return (T)bFormatter.Deserialize(stream);
        }
    }
}
