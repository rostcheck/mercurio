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

        public IEnumerable<IContainer> GetContainers()
        {
            var serializer = SerializerFactory.Create(_serializerType);
            var returnList = new List<IContainer>();
            var containerPaths = Directory.GetFiles(_path, "*.mcn", SearchOption.AllDirectories);
            foreach (var containerPath in containerPaths)
            {
                returnList.Add(DiskContainer.CreateFrom(containerPath, serializer));
            }

            return returnList;
        }

        public IContainer CreateContainer(string containerName, IStoragePlan storagePlan, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
        {            
            return DiskContainer.Create(_path, containerName, storagePlan, SerializerFactory.Create(_serializerType), retentionPolicy);
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
