using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyRingService
{
    public interface IKeyRingService
    {
        List<KeyRing> GetAvailableKeyRings();
        void AddKeyRing(KeyRing newKeyRing);
        KeyRing CreateKeyRing(string name);
    }
}
