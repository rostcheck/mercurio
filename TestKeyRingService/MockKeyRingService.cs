using KeyRingService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestKeyRingService
{
    public class MockKeyRingService : IKeyRingService
    {
        private List<KeyRing> keyringList;

        public MockKeyRingService()
        {
            keyringList = new List<KeyRing>();
            keyringList.Add(new KeyRing() { Name = "Alice", Path = @"c:\TestKeyRings\Alice" });
            keyringList.Add(new KeyRing() { Name = "Bob", Path = @"c:\TestKeyRings\Bob" });
        }

        public List<KeyRing> GetAvailableKeyRings()
        {
            return new List<KeyRing>(keyringList);
        }

        public void AddKeyRing(KeyRing newKeyRing)
        {
            keyringList.Add(newKeyRing);
        }

        public KeyRing CreateKeyRing(string name, NetworkCredential credential)
        {
            var newKeyRing = new KeyRing() { Name = name, Path = string.Format(@"c:\TestKeyRings\{0}", name) };
            keyringList.Add(newKeyRing);
            return newKeyRing;
        }
    }
}
