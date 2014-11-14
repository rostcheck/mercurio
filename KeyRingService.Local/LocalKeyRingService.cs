using Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            throw new NotImplementedException();
        }

        public KeyRing CreateKeyRing(string name)
        {
            throw new NotImplementedException();
        }
    }
}
