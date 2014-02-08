using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Encapsulates interaction with the user interface for use by lower layer subsystems
    /// </summary>
    public interface IMercurioUserAgent
    {
        void DisplayMessage(IMercurioMessage message);
        string GetSelectedIdentity();
        void InvalidMessageReceived(object message);
    }
}
