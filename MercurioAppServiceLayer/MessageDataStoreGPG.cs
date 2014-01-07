using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace MercurioAppServiceLayer
{
    public class MessageDataStoreGPG : IMessageDataStore
    {
        private Dictionary<string, List<IMercurioMessage>> store;
        private bool locked = true;

        ~MessageDataStoreGPG()
        {
            if (!locked)
            {
                throw new DataStoreException("Data store was not locked before destruction!");
            }
        }

        public bool IsLocked
        {
            get { return locked; }
        }

        public AppCryptoManagerType CryptoType
        {
            get { return AppCryptoManagerType.GPG; }
        }

        public bool Unlock(ICryptoManager cryptoManager, string passphrase)
        {
            throw new NotImplementedException();
        }

        public void Lock(ICryptoManager cryptoManager, string passphrase)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();            
            formatter.Serialize(stream, store);

            //cryptoManager.Encrypt(stream., EncryptionAlgorithmEnum.
        }

        public List<Entities.IMercurioMessage> GetMessages(string identifier)
        {
            if (store.ContainsKey(identifier))
            {
                return store[identifier];
            }
            else
            {
                return null;
            }
        }
    }
}
