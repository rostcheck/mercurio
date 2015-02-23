using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    /// <summary>
    /// Metadata about the Container that is encrypted when stored onto the StorageSubstrate
    /// </summary>
    [DataContract]
    [Serializable]
    public class DiskContainerPrivateMetadata : ISerializable
    {
        private const string ContainerNameSerializationName = "ContainerName";
        private const string ContainerDescriptionSerializationName = "ContainerDescription";

        [DataMember]
        public string ContainerName { get; private set; }
        [DataMember]
        public string ContainerDescription { get; private set; }

        // Needed for serialization
        protected DiskContainerPrivateMetadata(SerializationInfo info, StreamingContext context)
        {
            this.ContainerName = info.GetString(ContainerNameSerializationName);
            this.ContainerDescription = info.GetString(ContainerDescriptionSerializationName);
        }

        public DiskContainerPrivateMetadata(string name, string description)
        {
            this.ContainerName = name;
            this.ContainerDescription = description;
        }

        public static DiskContainerPrivateMetadata Create(string name, string description)
        {
            return new DiskContainerPrivateMetadata(name, description);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(ContainerNameSerializationName, ContainerName);
            info.AddValue(ContainerDescriptionSerializationName, ContainerDescription);
        }
    }
}
