using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// General representation of a serializer
    /// </summary>
    public interface ISerializerCore
    {
        void Serialize(Stream stream, object objectToSerialize);
        T Deserialize<T>(Stream stream);
    }
}
