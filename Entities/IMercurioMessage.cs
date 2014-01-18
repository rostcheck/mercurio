using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public interface IMercurioMessage : ISerializable
    {
        string SenderAddress { get; }
        string RecipientAddress { get; }
        bool Encryptable { get; } // invitations are not encryptable
        string Content { get; }
    }
}
