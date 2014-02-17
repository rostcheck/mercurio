using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Mercurio
{
    public class ConnectInvitationMessageViewModel : ViewModelBase
    {
        private ConnectInvitationMessage message;
        private bool isReviewed = true;
        private string senderAddress;
        private Uri evidenceUri;
        private string fingerprint;

        public ConnectInvitationMessageViewModel(ConnectInvitationMessage message, string fingerprint)
        {
            senderAddress = message.SenderAddress;
            evidenceUri = new Uri(message.Evidence);
            this.fingerprint = fingerprint;
            this.message = message;
        }

        public ConnectInvitationMessage Message
        {
            get { return message; }
        }

        public string SenderAddress
        {
            get
            {
                return senderAddress;
            }
        }

        public bool IsReviewed
        {
            get
            {
                return isReviewed;
            }
            set
            {
                if (value != isReviewed)
                {
                    value = isReviewed;
                    RaisePropertyChangedEvent("IsReviewed");
                }
            }
        }

        public string EvidenceURL
        {
            get
            {
                return evidenceUri.ToString();
            }
        }

        public string KeyID
        {
            get
            {
                return message.KeyID ?? string.Empty;
            }
        }

        public string Fingerprint
        {
            get
            {
                return fingerprint;
            }
        }
    }
}
