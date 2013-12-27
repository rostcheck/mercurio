using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Messages
{
    public class SignedKeyMessageProcessor :IMercurioMessageProcessor
    {
        public string MessageTypeName
        {
            get
            {
                return "Entities.SignedKeyMessage";
            }
        }

        public IMercurioMessage ProcessMessage(IMercurioMessage message, ICryptoManager cryptoManager, IMercurioUI ui)
        {
            if (message.GetType() != typeof(SignedKeyMessage))
            {
                string messageTypeName = message.GetType().ToString();
                throw new Exception("Unknown message type " + message + " passed to SignedKeyMessageProcessor:ProcessMessage");
            }
            SignedKeyMessage thisMessage = message as SignedKeyMessage;
                
            string keyID = cryptoManager.ImportKey(thisMessage.SignedPublicKey);
            string fingerprint = cryptoManager.GetFingerprint(keyID);

            //TODO: further secure this protocol; insure message is expected
            return null;
        }
    }
}
