using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    // Represents everything a UI can do
    public interface IMercurioUI
    {
        bool AcceptInvitation(ConnectInvitationMessage invitationMessage, string fingerprint);
    }
}
