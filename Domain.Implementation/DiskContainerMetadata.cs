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
        private const string RevisionRetentionPolicyTypeSerializationName = "RevisionRetentionPolicyType"; //TODO: move to private metadata

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string KeyFingerprint { get; private set; }
        [DataMember]
        public long PrivateDataStart { get; private set; }
        [DataMember]
        public long PrivateDataLength { get; private set; }
        [DataMember]
        public int RevisionRetentionPolicyType { get; private set; }

        // Needed for serialization
        protected DiskContainerMetadata(SerializationInfo info, StreamingContext context)
        {
            this.Name = info.GetString(NameSerializationName);
            this.KeyFingerprint = info.GetString(KeyFingerprintSerializationName);
            this.PrivateDataStart = info.GetInt32(PrivateDataStartSerializationName);
            this.PrivateDataLength = info.GetInt32(PrivateDataLengthSerializationName);
            this.RevisionRetentionPolicyType = info.GetInt32(RevisionRetentionPolicyTypeSerializationName);
        }

        public DiskContainerMetadata(string name, RevisionRetentionPolicyType retentionPolicyType)
        {
            this.Name = name;
            this.KeyFingerprint = "";
            this.PrivateDataStart = 0;
            this.PrivateDataLength = 0;
            this.RevisionRetentionPolicyType = (int)retentionPolicyType;
        }

        public static DiskContainerMetadata Create(string name, RevisionRetentionPolicyType revisionRetentionPolicyType)
        {
            return new DiskContainerMetadata(name, revisionRetentionPolicyType);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(NameSerializationName, this.Name);
            info.AddValue(KeyFingerprintSerializationName, KeyFingerprint);
            info.AddValue(PrivateDataStartSerializationName, PrivateDataStart);
            info.AddValue(PrivateDataLengthSerializationName, PrivateDataLength);
            info.AddValue(RevisionRetentionPolicyTypeSerializationName, RevisionRetentionPolicyType);
        }
    }
}
