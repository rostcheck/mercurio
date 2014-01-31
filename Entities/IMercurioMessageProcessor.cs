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

        /// <summary>
        /// Can return a response or null
        /// </summary>
        /// <param name="message">Incoming message</param>
        /// <param name="cryptoManager">Crypto manager</param>
        /// <param name="ui">Reference to UI</param>
        /// <returns>A repsonse message or null</returns>
        IMercurioMessage ProcessMessage(IMercurioMessage message, ICryptoManager cryptoManager, IMercurioUserAgent ui, Serializer serializer);
    }
}
