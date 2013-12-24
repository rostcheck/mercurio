using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    // Implemented by things that know how to process a specific type of IMercurioMessage
    public interface IMercurioMessageProcessor
    {
        string MessageTypeName { get; }
        void ProcessMessage(IMercurioMessage message, ICryptoManager cryptoManager, IMercurioUI ui, IPersistentQueue queue);
    }
}
