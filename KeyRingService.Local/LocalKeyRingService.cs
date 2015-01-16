using Entities;
using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KeyRingService.Local
{
    public class LocalKeyRingService : IKeyRingService
    {
        private ICryptoManager cryptoManager;
        private string keyRingDir;
 
        public LocalKeyRingService(ICryptoManager cryptoManager, string keyRingDir)
        {
            this.cryptoManager = cryptoManager;
            this.keyRingDir = keyRingDir;
        }

        public List<KeyRing> GetAvailableKeyRings()
        {
            var availableKeys = new List<KeyRing>();
            foreach (var dirName in Directory.EnumerateDirectories(keyRingDir))
            {
                availableKeys.Add(new KeyRing() { Name = Path.GetFileNameWithoutExtension(dirName), Path = dirName });
            }
            return availableKeys;
        }

        public void AddKeyRing(KeyRing newKeyRing)
        {
            try {
                // Verify the crypto manager recognizes that key ring dir as a valid key ring
                var identities = cryptoManager.GetAvailableIdentities();
                if (identities == null)
                {
                    throw new MercurioException(string.Format("Key ring at {0} contains no identities", newKeyRing.Path));
                }
            }
            catch (Exception e)
            {
                throw new MercurioException(string.Format("Error opening key ring at {0}", newKeyRing.Path), e);
            }
        }

        public KeyRing CreateKeyRing(string name, NetworkCredential credential)
        {
            //cryptoManager.CreateKey(name, credential);
            //TODO: implement
            throw new NotImplementedException();
        }
    }
}
