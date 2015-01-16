using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Mercurio.Domain;

namespace Mercurio
{
    public class IdentityViewModel : ViewModelBase
    {
        private Identity identity;
        private int numberOfUnreadMessages;

        public IdentityViewModel(Identity identity)
        {
            this.identity = identity;
        }

        public string Name
        {
            get
            {
                return identity.Name;
            }
        }

        public string Address
        {
            get
            {
                return identity.Address;
            }
        }

        public string Identifier
        {
            get
            {
                return identity.UniqueIdentifier;
            }
        }

        public int NumberOfUnreadMessages
        {
            get
            {
                return numberOfUnreadMessages;
            }
            set
            {
                numberOfUnreadMessages = value;
                RaisePropertyChangedEvent("NumberOfUnreadMessage");
                RaisePropertyChangedEvent("NameAndNumberUnread");
            }
        }

        public string NameAndNumberUnread
        {
            get
            {
                return numberOfUnreadMessages > 0 ? Name + " (" + NumberOfUnreadMessages + ")" : Name;
            }
        }
    }
}
