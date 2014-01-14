using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    public class Serializer
    {
        private ISerializerCore core;

        public Serializer(ISerializerCore core)
        {
            if (core == null)
                throw new ArgumentException("Must initialize Serializer with a valid SerializerCore");

            this.core = core;
        }

        public void Serialize(string filename, object objectToSerialize)
        {
            Stream stream = File.Open(filename, FileMode.Create);
            Serialize(stream, objectToSerialize);
            stream.Close();
        }

        public void Serialize(Stream stream, object objectToSerialize)
        {
            this.core.Serialize(stream, objectToSerialize);
        }

        public T Deserialize<T>(string filename)
        {
            Stream stream = File.Open(filename, FileMode.Open);
            T returnVal = Deserialize<T>(stream);
            stream.Close();
            return returnVal;
        }

        public T Deserialize<T>(Stream stream)
        {
            return (T)core.Deserialize<T>(stream);
        }
    }
}
