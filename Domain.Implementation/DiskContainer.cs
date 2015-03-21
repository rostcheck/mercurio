using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    /// <summary>
    /// A DiskContainer is essentially a private filesystem. It contains a public metadata section, 
    /// a private metadata section, a directory section, and a data section. All sections other than 
    /// the public metadata are encrypted.
    /// </summary>
    public class DiskContainer : Container, IContainer
    {
        private string _storagePath;
        private Serializer _serializer;

        protected DiskContainer(string storagePath, string containerName, Serializer serializer, ICryptoManager cryptoManager,
            RevisionRetentionPolicyType retentionPolicyType)
            : base(containerName, cryptoManager, retentionPolicyType)
        {
            _storagePath = storagePath;
            _serializer = serializer;
        }

        protected DiskContainer(ContainerMetadata metadata, Serializer serializer, string id, string diskPath)
            : base(metadata)
        {
            _serializer = serializer;
            this.Id = id;
            _storagePath = Path.GetDirectoryName(diskPath);
        }

        public static DiskContainer Create(string diskStorageSubstratePath, string containerName, Serializer serializer,
            ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
        {
            if (cryptoManager.GetActiveIdentity() == string.Empty)
            {
                throw new MercurioExceptionIdentityNotSet("Identity not set on cryptoManager");
            }

            var container = new DiskContainer(diskStorageSubstratePath, containerName, serializer, cryptoManager, retentionPolicy);
            container.EnsureDiskRepresentationExists();

            return container;
        }

        /// <summary>
        /// Create a DiskContainer from an (existing) disk representation. The container is locked (only public metadata is loaded)
        /// </summary>
        public static DiskContainer CreateFrom(string folderPath, Serializer serializer)
        {
            var id = Path.GetFileName(folderPath);
            var metadata = LoadMetadata(GetMetadataFilePath(folderPath, id), serializer);

            return new DiskContainer(metadata, serializer, id, folderPath);
        }

        public string FolderName
        {
            get
            {
                return Path.Combine(_storagePath, Id);
            }
        }

        public string MetadataFilePath
        {
            get
            {
                return GetMetadataFilePath(FolderName, Id);
            }
        }

        private static string GetMetadataFilePath(string folderName, string id)
        {
            return Path.Combine(folderName, string.Format("{0}.mcn", id));
        }

        public string PrivateMetadataFilePath
        {
            get
            {
                return GetPrivateMetadataFilePath(FolderName, Id);
            }
        }

        private static string GetPrivateMetadataFilePath(string folderName, string id)
        {
            return Path.Combine(folderName, string.Format("{0}.mc0", id));
        }

        private void EnsureDiskRepresentationExists()
        {
            if (!Directory.Exists(this.FolderName))
            {
                Directory.CreateDirectory(this.FolderName);
            }
            _serializer.Serialize(MetadataFilePath, _metadata);

            MemoryStream stream = new MemoryStream();
            _serializer.Serialize(stream, _privateMetadata);
            stream.Flush();
            stream.Position = 0;
            var encryptedStream =  _cryptoManager.Encrypt(stream, _metadata.KeyFingerprint);
            stream.Close();

            using (var diskStream = File.Create(PrivateMetadataFilePath))
            {
                encryptedStream.Position = 0;
                encryptedStream.CopyTo(diskStream);
            }
        }

        public override DocumentVersion CreateTextDocument(string documentName, Identity creatorIdentity, string initialData = null)
        {
            var document = base.CreateTextDocument(documentName, creatorIdentity, initialData);
            string filename = Path.Combine(FolderName, string.Format("{0}.mdc.1", Guid.NewGuid().ToString()));
            // TODO: Create the disk file and serialize the data to it   
   
            // TODO: apply revision management
            return document;
        }

        public override void Unlock(ICryptoManager cryptoManager)
        {
            _privateMetadata = LoadPrivateMetadata(PrivateMetadataFilePath, _serializer, cryptoManager);
            //base.Unlock(cryptoManager, privateMetadataBytes, serializer);
        }

        private static string GetPath(string directoryName, string id)
        {
            return Path.Combine(directoryName, id);
        }

        private static ContainerMetadata LoadMetadata(string filePath, Serializer serializer)
        {
            return serializer.Deserialize<ContainerMetadata>(filePath);
        }

        private static ContainerPrivateMetadata LoadPrivateMetadata(string diskPath, Serializer serializer, ICryptoManager cryptoManager)
        {
            var fileStream = File.OpenRead(diskPath);
            var privateMetadata = serializer.Deserialize<ContainerPrivateMetadata>(cryptoManager.Decrypt(fileStream));
            fileStream.Close();
            return privateMetadata;
        }

        private static void VerifyDiskRepresentationIntegrity(string folderName)
        {
            if (!Directory.Exists(folderName))
            {
                throw new MercurioException(string.Format("Container located at {0} is missing", folderName));
            }
            //TODO: check individual files exist
        }       
    }
}
