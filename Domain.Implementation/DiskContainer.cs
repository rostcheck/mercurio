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

        protected DiskContainer(string storagePath, string containerName, Serializer serializer, ICryptographicServiceProvider cryptoProvider,
            RevisionRetentionPolicyType retentionPolicyType)
            : base(containerName, retentionPolicyType)
        {
            _storagePath = storagePath;
            _cryptoManager = cryptoProvider.CreateManager(cryptoProvider.GetConfiguration());
            _metadata = DiskContainerMetadata.Create(containerName, cryptoProvider.GetProviderType(), retentionPolicyType);
            _privateMetadata = new DiskContainerPrivateMetadata();
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
                return Path.Combine(FolderName, string.Format("{0}.mcn", Id));
            }
        }

        public string PrivateMetadataFilePath
        {
            get
            {
                return Path.Combine(FolderName, string.Format("{0}.mc0", Id));
            }
        }

        protected DiskContainer(DiskContainerMetadata metadata, Serializer serializer, string id, string diskPath, ICryptographicServiceProvider cryptoProvider, RevisionRetentionPolicyType retentionPolicy)
            : base(metadata.Name, retentionPolicy)
        {
            _opened = false;
            _serializer = serializer;
            this.Id = id;
            var folderPath = Path.GetDirectoryName(diskPath);
            _storagePath = Path.GetDirectoryName(folderPath);
            _metadata = metadata;
            _cryptoManager = cryptoProvider.CreateManager(cryptoProvider.GetConfiguration());
        }

        public static DiskContainer Create(string diskStorageSubstratePath, string containerName, Serializer serializer, ICryptographicServiceProvider cryptoProvider,
            RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
        {
            var container = new DiskContainer(diskStorageSubstratePath, containerName, serializer, cryptoProvider, retentionPolicy);
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
            //_serializer.Serialize(PrivateMetadataFilePath, )
        }

        /// <summary>
        /// Create a DiskContainer from an (existing) disk representation
        /// </summary>
        public static DiskContainer CreateFrom(string diskPath, Serializer serializer, List<ICryptographicServiceProvider> availableCryptoProviders)
        {
            var metadata = LoadMetadata(serializer, diskPath);
            var cryptoProvider = availableCryptoProviders.Where(s => s.GetProviderType() == metadata.CryptoProviderType).FirstOrDefault();
            if (cryptoProvider == null)
            {
                var message = string.Format("Cannot open container located at path {0}; it requires a cryptographic provider of type {1} and none is installed", diskPath, metadata.CryptoProviderType);
                throw new MercurioExceptionRequiredCryptoProviderNotAvailable(message);
            }
            var id = Path.GetFileNameWithoutExtension(diskPath);
            var folderPath = Path.GetDirectoryName(diskPath);
            var storagePath = Path.GetDirectoryName(folderPath);
            return new DiskContainer(metadata, serializer, id, diskPath, cryptoProvider, (RevisionRetentionPolicyType)metadata.RevisionRetentionPolicyType);
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
            //set
            //{
            //    _metadata.RevisionRetentionPolicyType = value;
            //}
        }
        //public void Serialize(Serializer serializer)
        //{
        //    serializer.Serialize(_folderName, this);
        //}

        private static string GetPath(string directoryName, string id)
        {
            return Path.Combine(directoryName, id);
        }

        private static DiskContainerMetadata LoadMetadata(Serializer serializer, string filePath)
        {
            return serializer.Deserialize<DiskContainerMetadata>(filePath);
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
