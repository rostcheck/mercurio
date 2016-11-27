using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Mercurio.Domain;

namespace Mercurio.Domain.Implementation
{
    class PersistentQueueWithLocalFileStorage : IPersistentQueue
    {
        private Serializer _serializer;
		private string _name;
		private string _queueFileName;

        public PersistentQueueWithLocalFileStorage(PersistentQueueConfiguration configuration, Serializer serializer)
        {
			if (configuration == null)
				throw new ArgumentException("Must supply configuration to PersistentQueueWithLocalFileStorage");
			if (string.IsNullOrWhiteSpace(configuration.Name))
				throw new ArgumentException("Must supply queue name to PersistentQueueWithLocalFileStorage");
			if (string.IsNullOrWhiteSpace(configuration.ServiceType) || configuration.ServiceType != PersistentQueueType.LocalFileStorage)				
				throw new ArgumentException("Queue type for PersistentQueueWithLocalFileStorage must be " + PersistentQueueType.LocalFileStorage);
			if (string.IsNullOrWhiteSpace(configuration.ConfigurationString))
				throw new ArgumentException("Must supply queue configuration to PersistentQueueWithLocalFileStorage");

            this._serializer = serializer;
			this._name = configuration.Name;
			this._queueFileName = configuration.ConfigurationString;

            // Verify we could access this queue
            if (Directory.Exists(this._queueFileName))
            {
                var filePath = Path.Combine(this._queueFileName, "tstfile.tst");
                var stream = File.Create(filePath);
                stream.Close();
                File.Delete(filePath); 
            }
                
			else
				Directory.CreateDirectory(this._queueFileName);			
        }

		public string Name
		{
			get
			{
				return _name;
			}
		}

        public void Add(EnvelopedMercurioMessage message)
        {
            _serializer.Serialize(Path.Combine(_queueFileName, Guid.NewGuid().ToString(), ".msg"), message);
        }

        public EnvelopedMercurioMessage GetNext(string address)
        {
            var dirInfo = new DirectoryInfo(_queueFileName);
            var fileInfo = dirInfo.GetFiles("*.msg").OrderBy(p => p.CreationTime);
            if (fileInfo.Count() == 0)
            {
                return null; // queue empty
            }
            else
            {
                string fileName = Path.Combine(_queueFileName, Guid.NewGuid().ToString(), ".msg");
                var message = _serializer.Deserialize<EnvelopedMercurioMessage>(fileName);
                File.Delete(fileName);
                return message;
            }
        }

        public int Length(string address)
        {
            return Directory.GetFiles(_queueFileName, "*.msg").Length;
        }
    }
}
