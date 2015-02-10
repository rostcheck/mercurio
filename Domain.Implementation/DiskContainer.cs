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
        private string _folderName;
        private DiskContainerMetadata _metadata;
        private DiskContainerPrivateMetadata _privateMetadata;
        private Dictionary<string, DiskDirectoryNode> _directory;
        private Serializer _serializer;
        private bool _opened;

        protected DiskContainer(string folderName, string containerName, IStoragePlan storagePlan, Serializer serializer,
            RevisionRetentionPolicyType retentionPolicy)
            : base(containerName, storagePlan, retentionPolicy)
        {
            _folderName = folderName;
            _metadata = new DiskContainerMetadata();
            _privateMetadata = new DiskContainerPrivateMetadata();
            _directory = new Dictionary<string, DiskDirectoryNode>();
            _serializer = serializer;
            _opened = true;
        }

        protected DiskContainer(string diskPath)
        {
            _opened = false;
        }

        public static DiskContainer Create(string diskStorageSubstratePath, string containerName, IStoragePlan storagePlan,
            Serializer serializer, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
        {
            string folderName = Path.Combine(diskStorageSubstratePath, Guid.NewGuid().ToString());
            if (!Directory.Exists(folderName))
            {
                CreateDiskRepresentation(folderName);
            }
            VerifyDiskRepresentationIntegrity(folderName);

            return new DiskContainer(folderName, containerName, storagePlan, serializer, retentionPolicy);
        }

        /// <summary>
        /// Create a DiskContainer from an (existing) disk representation
        /// </summary>
        public static DiskContainer CreateFrom(string diskPath)
        {
            return new DiskContainer(diskPath);
        }

        public override TextDocument CreateTextDocument(string documentName, Identity creatorIdentity, string initialData = null)
        {
            var document = base.CreateTextDocument(documentName, creatorIdentity, initialData);
            string filename = Path.Combine(_folderName, string.Format("{0}.mdc.1", Guid.NewGuid().ToString()));
            // TODO: Create the disk file and serialize the data to it            
            _directory.Add(document.Name.ToLower(), DiskDirectoryNode.Create(filename, 1));
            return document;
        }

        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(_folderName, this);
        }

        private static void CreateDiskRepresentation(string folderName)
        {
            Directory.CreateDirectory(folderName);
            //TODO: initialize files
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
