﻿using System;
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
        private Dictionary<string, DiskDirectoryNode> _directory;
        private Serializer _serializer;

        protected DiskContainer(string storagePath, string containerName, Serializer serializer, ICryptoManager cryptoManager,
            RevisionRetentionPolicyType retentionPolicyType)
            : base(containerName, cryptoManager, retentionPolicyType)
        {
            _storagePath = storagePath;
            _directory = new Dictionary<string, DiskDirectoryNode>();
            _serializer = serializer;
        }

        protected DiskContainer(ContainerMetadata metadata, Serializer serializer, string id, string diskPath)
            : base(metadata)
        {
            _serializer = serializer;
            this.Id = id;
            var folderPath = Path.GetDirectoryName(diskPath);
            _storagePath = Path.GetDirectoryName(folderPath);
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
            var encryptedStream =  _cryptoManager.Encrypt(stream, _metadata.KeyFingerprint);

            FileStream diskStream = File.Open(PrivateMetadataFilePath, FileMode.Create);
            StreamWriter diskWriter = new StreamWriter(diskStream);
            diskWriter.Write(encryptedStream);
            diskWriter.Flush();
            diskWriter.Close();
        }

        public override TextDocument CreateTextDocument(string documentName, Identity creatorIdentity, string initialData = null)
        {
            var document = base.CreateTextDocument(documentName, creatorIdentity, initialData);
            string filename = Path.Combine(FolderName, string.Format("{0}.mdc.1", Guid.NewGuid().ToString()));
            // TODO: Create the disk file and serialize the data to it            
            _directory.Add(document.Name.ToLower(), DiskDirectoryNode.Create(filename, 1));
            return document;
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
