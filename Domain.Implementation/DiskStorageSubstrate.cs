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

		private DiskStorageSubstrate(string diskPath, SerializerType serializerType, bool isDefault) :
			base(isDefault)
        {
            _path = diskPath;
            _serializerType = serializerType;
        }

		public static DiskStorageSubstrate Create(string diskPath, SerializerType serializerType, bool isDefault = false)
        {
            if (diskPath == null || diskPath == "")
                throw new ArgumentNullException("Path cannot be null");

            if (!Directory.Exists(diskPath))
                throw new DirectoryNotFoundException("Directory " + diskPath + " was not found");

            return new DiskStorageSubstrate(diskPath, serializerType, isDefault);
        }

        // Create a new independent substrate accessor from this one
        public static DiskStorageSubstrate Create(DiskStorageSubstrate substrate)
        {
            return DiskStorageSubstrate.Create(substrate._path, substrate._serializerType);
        }

        public string Name
        {
            get { return Path.GetFileName(_path); }
        }

        public List<IContainer> GetAllContainers()
        {
            var serializer = SerializerFactory.Create(_serializerType);
            var returnList = new List<IContainer>();
            var containerPaths = Directory.GetDirectories(_path);
            foreach (var containerPath in containerPaths)
            {
                var containerId = Path.GetFileName(containerPath);
                var metadata = RetrieveMetadata(GetMetadataFilePath(containerId), serializer);
                var container = Container.CreateFrom(metadata, Guid.Parse(containerId), DiskStorageSubstrate.Create(this), serializer);
                returnList.Add(container);
            }

            return returnList;
        }

        public DocumentVersion RetrieveDocumentVersion(Guid containerId, DocumentVersionMetadata documentVersionMetadata)
        {
            var fileContent = File.ReadAllText(GetDocumentVersionPath(containerId, documentVersionMetadata.DocumentId, documentVersionMetadata.Id));
            return DocumentVersion.Create(documentVersionMetadata.DocumentId, documentVersionMetadata.PriorVersionId, documentVersionMetadata.CreatedDateTime.UtcTicks, documentVersionMetadata.CreatorId, fileContent, documentVersionMetadata.IsDeleted);
        }

        public void StoreDocumentVersion(Guid containerId, DocumentVersion documentVersion)
        {
            var documentPath = GetDocumentPath(containerId, documentVersion.DocumentId);
            if (!Directory.Exists(documentPath))
                Directory.CreateDirectory(documentPath);
            File.WriteAllText(GetDocumentVersionPath(containerId, documentVersion.DocumentId, documentVersion.Id), documentVersion.DocumentContent);
        }

        public void DeleteDocumentVersion(Guid containerId, DocumentVersionMetadata documentVersionMetadata)
        {
            File.Delete(GetDocumentVersionPath(containerId, documentVersionMetadata.DocumentId, documentVersionMetadata.Id));
            // If we deleted the last version, remove the directory too
            var documentPath = GetDocumentPath(containerId, documentVersionMetadata.DocumentId);
            if (Directory.EnumerateFiles(documentPath).ToList().Count == 0)
                Directory.Delete(documentPath);
        }

        public bool HostsContainer(Guid containerId)
        {
            return (GetContainerPath(containerId.ToString()) != null);
        }

        private string GetContainerPath(string containerId)
        {
            return Path.Combine(_path, containerId);
        }

        private string GetMetadataFilePath(string containerId)
        {
            return Path.Combine(GetContainerPath(containerId), string.Format("{0}.mcn", containerId));
        }

        private string GetPrivateMetadataFilePath(string containerId)
        {
            return Path.Combine(GetContainerPath(containerId), string.Format("{0}.mc0", containerId));
        }

        public string GetDocumentPath(Guid containerId, Guid documentId)
        {
            return Path.Combine(GetContainerPath(containerId.ToString()), documentId.ToString());
        }

        public string GetDocumentVersionPath(Guid containerId, Guid documentId, Guid versionId)
        {
            return Path.Combine(GetDocumentPath(containerId, documentId), versionId.ToString());
        }

        public byte[] RetrievePrivateMetadataBytes(Guid containerId)
        {
            var diskPath = GetPrivateMetadataFilePath(containerId.ToString());
            if (diskPath == null || diskPath == string.Empty)
                throw new MercurioException(string.Format("Container with id {0} is not hosted on this substrate ({1})", containerId.ToString(), this.Name));

            return File.ReadAllBytes(diskPath);
        }

        private ContainerMetadata RetrieveMetadata(string filePath, Serializer serializer)
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
            var serializer = SerializerFactory.Create(_serializerType);
            return Container.Create(containerName, cryptoManager, DiskStorageSubstrate.Create(this), serializer, retentionPolicy);
        }

        public void DeleteContainer(Guid containerId)
        {
            var folderName = GetFolderName(containerId);
            if (Directory.Exists(folderName))
                Directory.Delete(folderName, true);
        }

        public void StoreMetadata(Guid containerId, ContainerMetadata metadata)
        {
            var serializer = SerializerFactory.Create(_serializerType);
            var folderName = GetFolderName(containerId);
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
            serializer.Serialize(GetMetadataFilePath(folderName, containerId), metadata);
        }

        private string GetFolderName(Guid containerId)
        {
            return Path.Combine(_path, containerId.ToString());
        }

        private static string GetMetadataFilePath(string folderName, Guid containerId)
        {
            return Path.Combine(folderName, string.Format("{0}.mcn", containerId.ToString()));
        }

        private static string GetPrivateMetadataFilePath(string folderName, string id)
        {
            return Path.Combine(folderName, string.Format("{0}.mc0", id));
        }

        public void StorePrivateMetadata(Guid containerId, Stream encryptedPrivateMetadata)
        {
            var folderName = GetFolderName(containerId);
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
            var path = GetPrivateMetadataFilePath(GetFolderName(containerId), containerId.ToString());
            using (var fileStream = File.Create(path))
            {
                encryptedPrivateMetadata.Seek(0, SeekOrigin.Begin);
                encryptedPrivateMetadata.CopyTo(fileStream);
            }
        }
    }
}
