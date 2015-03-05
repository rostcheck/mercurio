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
        private DiskContainerMetadata _metadata;
        private DiskContainerPrivateMetadata _privateMetadata;
        private Dictionary<string, DiskDirectoryNode> _directory;
        private Serializer _serializer;
        private string _id;
        private bool _opened;
        private ICryptoManager _cryptoManager;

        protected DiskContainer(string storagePath, string containerName, string keyFingerprint, Serializer serializer, ICryptoManager cryptoManager,
            RevisionRetentionPolicyType retentionPolicyType)
            : base(containerName, retentionPolicyType)
        {
            _storagePath = storagePath;
            _cryptoManager = cryptoManager;
            _metadata = DiskContainerMetadata.Create(containerName, cryptoManager.ManagerType, keyFingerprint, retentionPolicyType);
            _privateMetadata = DiskContainerPrivateMetadata.Create(containerName, "");
            _directory = new Dictionary<string, DiskDirectoryNode>();
            _serializer = serializer;
            Id = Guid.NewGuid().ToString();
            _opened = true;
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

        protected DiskContainer(DiskContainerMetadata metadata, Serializer serializer, string id, string diskPath, 
            ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicy)
            : base(metadata.Name, retentionPolicy)
        {
            _opened = false;
            _serializer = serializer;
            this.Id = id;
            var folderPath = Path.GetDirectoryName(diskPath);
            _storagePath = Path.GetDirectoryName(folderPath);
            _metadata = metadata;
            _cryptoManager = cryptoManager;
        }

        public static DiskContainer Create(string diskStorageSubstratePath, string containerName, string keyFingerprint, Serializer serializer, 
            ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
        {
            var container = new DiskContainer(diskStorageSubstratePath, containerName, keyFingerprint, serializer, cryptoManager, retentionPolicy);
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
        /// Create a DiskContainer from an (existing) disk representation
        /// </summary>
        public static DiskContainer CreateFrom(string folderPath, Serializer serializer, List<ICryptographicServiceProvider> availableCryptoProviders)
        {
            var id = Path.GetFileName(folderPath);
            var metadata = LoadMetadata(GetMetadataFilePath(folderPath, id), serializer);
            var cryptoProvider = availableCryptoProviders.Where(s => s.GetProviderType() == metadata.CryptoProviderType).FirstOrDefault();
            if (cryptoProvider == null)
            {
                var message = string.Format("Cannot open container located at path {0}; it requires a cryptographic provider of type {1} and none is installed", folderPath, metadata.CryptoProviderType);
                throw new MercurioExceptionRequiredCryptoProviderNotAvailable(message);
            }

            var privateMetadata = LoadPrivateMetadata(GetPrivateMetadataFilePath(folderPath, id), serializer, cryptoProvider);
            var cryptoManager = cryptoProvider.CreateManager(cryptoProvider.GetConfiguration());
            return new DiskContainer(metadata, serializer, id, folderPath, cryptoManager, (RevisionRetentionPolicyType)metadata.RevisionRetentionPolicyType);
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

        private static DiskContainerMetadata LoadMetadata(string filePath, Serializer serializer)
        {
            return serializer.Deserialize<DiskContainerMetadata>(filePath);
        }

        private static DiskContainerPrivateMetadata LoadPrivateMetadata(string diskPath, Serializer serializer, ICryptographicServiceProvider cryptoProvider)
        {
            var cryptoManager = cryptoProvider.CreateManager(cryptoProvider.GetConfiguration());
            var fileStream = File.OpenRead(diskPath);
            var privateMetadata = serializer.Deserialize<DiskContainerPrivateMetadata>(cryptoManager.Decrypt(fileStream));
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
