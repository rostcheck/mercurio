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
        private const string CryptoProviderTypeSerializationName = "CryptoProviderType";
        private const string RevisionRetentionPolicyTypeSerializationName = "RevisionRetentionPolicyType"; //TODO: move to private metadata

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string KeyFingerprint { get; private set; }
        [DataMember]
        public string CryptoProviderType { get; private set; }
        [DataMember]
        public int RevisionRetentionPolicyType { get; private set; }

        // Needed for serialization
        protected DiskContainerMetadata(SerializationInfo info, StreamingContext context)
        {
            this.Name = info.GetString(NameSerializationName);
            this.KeyFingerprint = info.GetString(KeyFingerprintSerializationName);
            this.CryptoProviderType = info.GetString(CryptoProviderTypeSerializationName);
            this.RevisionRetentionPolicyType = info.GetInt32(RevisionRetentionPolicyTypeSerializationName);
        }

        public DiskContainerMetadata(string name, string cryptoProviderType, RevisionRetentionPolicyType retentionPolicyType)
        {
            this.Name = name;
            this.KeyFingerprint = "";
            this.CryptoProviderType = cryptoProviderType;
            this.RevisionRetentionPolicyType = (int)retentionPolicyType;
        }

        public static DiskContainerMetadata Create(string name, string cryptoProviderType, RevisionRetentionPolicyType revisionRetentionPolicyType)
        {
            return new DiskContainerMetadata(name, cryptoProviderType, revisionRetentionPolicyType);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(NameSerializationName, this.Name);
            info.AddValue(KeyFingerprintSerializationName, KeyFingerprint);
            info.AddValue(CryptoProviderTypeSerializationName, CryptoProviderType);
            info.AddValue(RevisionRetentionPolicyTypeSerializationName, RevisionRetentionPolicyType);
        }
    }
}
