using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    [DataContract]
    [Serializable]
    public class DiskContainerMetadata : ISerializable
    {
        private const string NameSerializationName = "Name";
        private const string KeyFingerprintSerializationName = "KeyFingerprint";
        private const string PrivateDataStartSerializationName = "PrivateDataStart";
        private const string PrivateDataLengthSerializationName = "PrivateDataLength";

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string KeyFingerprint { get; private set; }
        [DataMember]
        public long PrivateDataStart { get; private set; }
        [DataMember]
        public long PrivateDataLength { get; private set; }

        // Needed for serialization
        protected DiskContainerMetadata(SerializationInfo info, StreamingContext context)
        {
            this.Name = info.GetString(NameSerializationName);
            this.KeyFingerprint = info.GetString(KeyFingerprintSerializationName);
            this.PrivateDataStart = info.GetInt32(PrivateDataStartSerializationName);
            this.PrivateDataLength = info.GetInt32(PrivateDataLengthSerializationName);
        }

        public DiskContainerMetadata(string name)
        {
            this.Name = name;
            this.KeyFingerprint = "";
            this.PrivateDataStart = 0;
            this.PrivateDataLength = 0;
        }

        public static DiskContainerMetadata Create(string name)
        {
            return new DiskContainerMetadata(name);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(NameSerializationName, this.Name);
            info.AddValue(KeyFingerprintSerializationName, KeyFingerprint);
            info.AddValue(PrivateDataStartSerializationName, PrivateDataStart);
            info.AddValue(PrivateDataLengthSerializationName, PrivateDataLength);
        }
    }
}
