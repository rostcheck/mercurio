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

        public string Name { get; set; }
        public string KeyFingerprint { get; private set; }
        public string CryptoProviderType { get; private set; }

        // Needed for serialization
        protected ContainerMetadata(SerializationInfo info, StreamingContext context)
        {
            this.Name = info.GetString(NameSerializationName);
            this.KeyFingerprint = info.GetString(KeyFingerprintSerializationName);
            this.CryptoProviderType = info.GetString(CryptoProviderTypeSerializationName);
        }

        public ContainerMetadata(string name, string cryptoProviderType, string keyFingerprint)
        {
            this.Name = name;
            this.KeyFingerprint = keyFingerprint;
            this.CryptoProviderType = cryptoProviderType;
        }

        public static ContainerMetadata Create(string name, string cryptoProviderType, string keyFingerprint)
        {
            return new ContainerMetadata(name, cryptoProviderType, keyFingerprint);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(NameSerializationName, Name);
            info.AddValue(KeyFingerprintSerializationName, KeyFingerprint);
            info.AddValue(CryptoProviderTypeSerializationName, CryptoProviderType);
        }
    }
}
