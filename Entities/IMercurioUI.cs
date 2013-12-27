﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    // Represents everything a UI can do
    public interface IMercurioUI
    {
        void DisplayTextMessage(string textMessage);
        string GetSelectedIdentity(ICryptoManager cryptoManager);
        bool AcceptInvitation(ConnectInvitationMessage invitationMessage, string fingerprint);
        bool AcceptInvitationResponse(ConnectInvitationAcceptedMessage invitationAcceptedMessage, string fingerprint);
    }
}
