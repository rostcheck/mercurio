using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    [Serializable]
    public class ContainerMetadata : ISerializable
    {
        private const string NameSerializationName = "Name";
        private const string KeyFingerprintSerializationName = "KeyFingerprint";
        private const string CryptoProviderTypeSerializationName = "CryptoProviderType";
        private const string RevisionRetentionPolicyTypeSerializationName = "RevisionRetentionPolicyType"; //TODO: move to private metadata

        public string Name { get; set; }
        public string KeyFingerprint { get; private set; }
        public string CryptoProviderType { get; private set; }
        public int RevisionRetentionPolicyType { get; private set; }

        // Needed for serialization
        protected ContainerMetadata(SerializationInfo info, StreamingContext context)
        {
            this.Name = info.GetString(NameSerializationName);
            this.KeyFingerprint = info.GetString(KeyFingerprintSerializationName);
            this.CryptoProviderType = info.GetString(CryptoProviderTypeSerializationName);
            this.RevisionRetentionPolicyType = info.GetInt32(RevisionRetentionPolicyTypeSerializationName);
        }

        public ContainerMetadata(string name, string cryptoProviderType, string keyFingerprint, RevisionRetentionPolicyType retentionPolicyType)
        {
            this.Name = name;
            this.KeyFingerprint = keyFingerprint;
            this.CryptoProviderType = cryptoProviderType;
            this.RevisionRetentionPolicyType = (int)retentionPolicyType;
        }

        public static ContainerMetadata Create(string name, string cryptoProviderType, string keyFingerprint, RevisionRetentionPolicyType revisionRetentionPolicyType)
        {
            return new ContainerMetadata(name, cryptoProviderType, keyFingerprint, revisionRetentionPolicyType);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(NameSerializationName, Name);
            info.AddValue(KeyFingerprintSerializationName, KeyFingerprint);
            info.AddValue(CryptoProviderTypeSerializationName, CryptoProviderType);
            info.AddValue(RevisionRetentionPolicyTypeSerializationName, RevisionRetentionPolicyType);
        }
    }
}
