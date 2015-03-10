using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Metadata about the Container that is encrypted when stored onto the StorageSubstrate
    /// </summary>
    [Serializable]
    public class ContainerPrivateMetadata : ISerializable
    {
        private const string ContainerNameSerializationName = "ContainerName";
        private const string ContainerDescriptionSerializationName = "ContainerDescription";

        public string ContainerName { get; private set; }
        public string ContainerDescription { get; private set; }

        // Needed for serialization
        protected ContainerPrivateMetadata(SerializationInfo info, StreamingContext context)
        {
            this.ContainerName = info.GetString(ContainerNameSerializationName);
            this.ContainerDescription = info.GetString(ContainerDescriptionSerializationName);
        }

        public ContainerPrivateMetadata(string name, string description)
        {
            this.ContainerName = name;
            this.ContainerDescription = description;
        }

        public static ContainerPrivateMetadata Create(string name, string description)
        {
            return new ContainerPrivateMetadata(name, description);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(ContainerNameSerializationName, ContainerName);
            info.AddValue(ContainerDescriptionSerializationName, ContainerDescription);
        }
    }
}
