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
        private ContainerMetadata _metadata;
        private ContainerPrivateMetadata _privateMetadata;
        private Dictionary<string, DiskDirectoryNode> _directory;
        private Serializer _serializer;
        private string _id;

        protected DiskContainer(string storagePath, string containerName, Serializer serializer, ICryptoManager cryptoManager,
            RevisionRetentionPolicyType retentionPolicyType)
            : base(containerName, cryptoManager, retentionPolicyType)
        {
            _storagePath = storagePath;
            _metadata = ContainerMetadata.Create(containerName, cryptoManager.ManagerType, cryptoManager.GetActiveIdentity(), retentionPolicyType);
            _privateMetadata = ContainerPrivateMetadata.Create(containerName, "");
            _directory = new Dictionary<string, DiskDirectoryNode>();
            _serializer = serializer;
            Id = Guid.NewGuid().ToString();
        }

        protected DiskContainer(ContainerMetadata metadata, ContainerPrivateMetadata privateMetadata, Serializer serializer, string id, string diskPath,
            RevisionRetentionPolicyType retentionPolicy)
            : base(metadata.Name, null, retentionPolicy)
        {
            _serializer = serializer;
            this.Id = id;
            var folderPath = Path.GetDirectoryName(diskPath);
            _storagePath = Path.GetDirectoryName(folderPath);
            _metadata = metadata;
            _privateMetadata = privateMetadata;
        }

        public string Id { get; protected set; }

        public override string Name
        {
            get
            {
                return _metadata.Name;
            }
            protected set
            {
                if (_metadata != null)
                {
                    _metadata.Name = value;
                }
            }
        }

        public override string CryptoManagerType
        {
            get
            {
                return _metadata.CryptoProviderType;
            }
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

        public override bool IsLocked
        {
            get
            {
                return _privateMetadata == null;
            }
        }

        public override bool IsAvailableToIdentity(string uniqueIdentifier)
        {
            return (_metadata.KeyFingerprint == uniqueIdentifier); // TODO: generalize to support multiple identities
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
            //VerifyDiskRepresentationIntegrity(folderName);

            return container;
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
            var encryptedStream =  _cryptoManager.Encrypt(stream, _metadata.KeyFingerprint);

            FileStream diskStream = File.Open(PrivateMetadataFilePath, FileMode.Create);
            StreamWriter diskWriter = new StreamWriter(diskStream);
            diskWriter.Write(encryptedStream);
            diskWriter.Flush();
            diskWriter.Close();
        }

        /// <summary>
        /// Create a DiskContainer from an (existing) disk representation. The container is locked (only public metadata is loaded)
        /// </summary>
        public static DiskContainer CreateFrom(string folderPath, Serializer serializer)
            //, List<ICryptographicServiceProvider> availableCryptoProviders)
        {
            var id = Path.GetFileName(folderPath);
            var metadata = LoadMetadata(GetMetadataFilePath(folderPath, id), serializer);
            //var cryptoProvider = availableCryptoProviders.Where(s => s.GetProviderType() == metadata.CryptoProviderType).FirstOrDefault();
            //if (cryptoProvider == null)
            //{
            //    var message = string.Format("Cannot open container located at path {0}; it requires a cryptographic provider of type {1} and none is installed", folderPath, metadata.CryptoProviderType);
            //    throw new MercurioExceptionRequiredCryptoProviderNotAvailable(message);
            //}

            //var privateMetadata = LoadPrivateMetadata(GetPrivateMetadataFilePath(folderPath, id), serializer, cryptoProvider);
            //var cryptoManager = cryptoProvider.CreateManager(cryptoProvider.GetConfiguration());
            return new DiskContainer(metadata, null, serializer, id, folderPath, (RevisionRetentionPolicyType)metadata.RevisionRetentionPolicyType);
        }

        /// <summary>
        /// Unlock the container (read its private metadata). Requires a cryptoManager w/ credentials set
        /// </summary>
        /// <param name="cryptoManager"></param>
        public override void Unlock(ICryptoManager cryptoManager)
        {
            throw new NotImplementedException();
        }

        public override TextDocument CreateTextDocument(string documentName, Identity creatorIdentity, string initialData = null)
        {
            var document = base.CreateTextDocument(documentName, creatorIdentity, initialData);
            string filename = Path.Combine(FolderName, string.Format("{0}.mdc.1", Guid.NewGuid().ToString()));
            // TODO: Create the disk file and serialize the data to it            
            _directory.Add(document.Name.ToLower(), DiskDirectoryNode.Create(filename, 1));
            return document;
        }

        protected override IRevisionRetentionPolicy RevisionRetentionPolicy
        {
            get
            {
                return Mercurio.Domain.RevisionRetentionPolicy.Create((RevisionRetentionPolicyType)_metadata.RevisionRetentionPolicyType);
            }
        }

        private static string GetPath(string directoryName, string id)
        {
            return Path.Combine(directoryName, id);
        }

        private static ContainerMetadata LoadMetadata(string filePath, Serializer serializer)
        {
            return serializer.Deserialize<ContainerMetadata>(filePath);
        }

        private static ContainerPrivateMetadata LoadPrivateMetadata(string diskPath, Serializer serializer, ICryptographicServiceProvider cryptoProvider)
        {
            var cryptoManager = cryptoProvider.CreateManager(cryptoProvider.GetConfiguration());
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
