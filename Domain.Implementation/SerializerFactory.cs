using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    public static class SerializerFactory
    {
        public static Serializer Create(SerializerType serializerType)
        {
            switch (serializerType)
            {
                case SerializerType.BinarySerializer:
                    BinarySerializerCore core = new BinarySerializerCore();
                    return new Serializer(core);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
