using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    public class DiskStorageSubstrate : IStorageSubstrate
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
            {
                throw new DirectoryNotFoundException();
            }

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
                returnList.Add(DiskContainer.CreateFrom(containerPath, serializer));
            }

            return returnList;
        }

        public IEnumerable<IContainer> GetAccessibleContainers(string identity, ICryptoManager cryptoManager)
        {
            return GetAllContainers().Where(s => (s.CryptoManagerType == cryptoManager.ManagerType && s.IsAvailableToIdentity(identity)));
        }

        public IContainer CreateContainer(string containerName, string keyFingerprint, ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
        {
            return DiskContainer.Create(_path, containerName, keyFingerprint, SerializerFactory.Create(_serializerType), cryptoManager, retentionPolicy);
        }

        public void CloseContainer (IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException();

            container.Lock();
        }

        private DiskContainerMetadata GetContainerMetadata(string path)
        {
            var serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            return serializer.Deserialize<DiskContainerMetadata>(path);
        }
    }
}
