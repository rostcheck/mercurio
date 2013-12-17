using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace TestEntities
{
    public class DummyMercurioUI : IMercurioUI
    {
        public bool AcceptInvitation(ConnectInvitationMessage invitationMessage)
        {
            return true;
        }
    }
}
