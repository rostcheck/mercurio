using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    public class DiskStorageSubstrate : StorageSubstrateBase, IStorageSubstrate
    {
        private string _path;
        private SerializerType _serializerType;

        private DiskStorageSubstrate(string diskPath, SerializerType serializerType)
        {
            _path = diskPath;
            _serializerType = serializerType;
        }

        public static DiskStorageSubstrate Create(string diskPath, SerializerType serializerType)
        {
            if (diskPath == null || diskPath == "")
                throw new ArgumentNullException("Path cannot be null");

            if (!Directory.Exists(diskPath))
                throw new DirectoryNotFoundException();

            return new DiskStorageSubstrate(diskPath, serializerType);
        }

        public string Name
        {
            get { return _path; }
        }

        public IEnumerable<IContainer> GetAllContainers()
        {
            var serializer = SerializerFactory.Create(_serializerType);
            var returnList = new List<IContainer>();
            var containerPaths = Directory.GetDirectories(_path);
            foreach (var containerPath in containerPaths)
            {
                var metadata = LoadMetadata(containerPath, serializer);
                var container = Container.CreateFrom(metadata);
                HookEvents(container);
                returnList.Add(container);
            }

            return returnList;
        }

        // Attach this substrate to the events the container publishes when it needs access to storage
        private void HookEvents(IContainer container)
        {
            container.StoreDocumentVersionEvent += container_StoreDocumentVersionEvent;
            container.RetrieveDocumentVersionEvent += container_RetrieveDocumentVersionEvent;
        }

        DocumentVersion container_RetrieveDocumentVersionEvent(Guid containerId, DocumentVersionMetadata documentVersionMetadata)
        {
            var fileContent = File.ReadAllText(GetDocumentVersionPath(containerId, documentVersionMetadata.DocumentId, documentVersionMetadata.Id));
            return DocumentVersion.Create(documentVersionMetadata.DocumentId, documentVersionMetadata.PriorVersionId, documentVersionMetadata.CreatorId, fileContent);
        }

        void container_StoreDocumentVersionEvent(Guid containerId, DocumentVersion documentVersion)
        {
            File.WriteAllText(GetDocumentVersionPath(containerId, documentVersion.DocumentId, documentVersion.Id), documentVersion.DocumentContent);
        }

        public bool HostsContainer(string containerId)
        {
            return (GetContainerPath(containerId) != null);
        }

        private string GetContainerPath(string containerId)
        {
            var containerPaths = Directory.GetDirectories(_path);
            foreach (var containerPath in containerPaths)
            {
                if (containerPath.Contains(containerId))
                    return containerPath;
            }
            return null;
        }

        public string GetDocumentPath(Guid containerId, Guid documentId)
        {
            return Path.Combine(GetContainerPath(containerId.ToString()), documentId.ToString());
        }

        public string GetDocumentVersionPath(Guid containerId, Guid documentId, Guid versionId)
        {
            return Path.Combine(GetDocumentPath(containerId, documentId), versionId.ToString());
        }

        public byte[] GetPrivateMetadataBytes(string containerId)
        {
            var diskPath = GetContainerPath(containerId);
            if (diskPath == null || diskPath == string.Empty)
                throw new MercurioException(string.Format("Container with id {0} is not hosted on this substrate ({1})", containerId, this.Name));

            return File.ReadAllBytes(diskPath);
        }

        private ContainerMetadata LoadMetadata(string filePath, Serializer serializer)
        {
            return serializer.Deserialize<ContainerMetadata>(filePath);
        }

        //public byte[] GetPrivateMetadataBytes(string containerId)
        //{
        //    var fileStream = File.OpenRead(diskPath);
        //    var privateMetadata = base.LoadPrivateMetadata(fileStream, serializer, cryptoManager);
        //    fileStream.Close();
        //    return privateMetadata;
        //}

        public IContainer CreateContainer(string containerName, ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
        {
            return Container.Create(containerName, cryptoManager, retentionPolicy);
        }
    }
}
