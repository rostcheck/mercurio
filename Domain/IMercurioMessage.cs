using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    // Notes on message processing:
    //
    // Each message, when Process() is called, should return a successor message for further 
    // processing or null to indicate it is fully processed.
    //
    // For example: 
    // - TimeDelayedMessage's Process() will wait the delay time, then return its content 
    //   message.
    // - EncryptedMercurioMessage's Process() will decrypt its Payload and return that (it 
    //   actually wraps it in a TimeDelayedMessage)
    // - SimpleTextMessage's Process() will return null.

    // If the message is suitable for display by a UI, it should raise the 
    // MessageIsDisplayableEvent (messages usually derive from MercurioMessageBase to provide 
    // the RaiseMessageIsDisplayable() method to do this). The UI may choose not to display 
    // the message.
    //
    // For example, a TimeDelayedMessage is not displayable, so does not raise 
    // MessageIsDisplayableEvent in its Process(), but the message it contains usually is, so 
    // in that message's Process() it would raise its own event.
    //

    public delegate void MessageIsDisplayable(IMercurioMessage message);

    public interface IMercurioMessage : ISerializable
    {
        Guid ContentID { get; }
        string SenderAddress { get; }
        string RecipientAddress { get; }
        bool Encryptable { get; } // invitations are not encryptable
        string Content { get; }

        /// <summary>
        /// Tell the message to process itself
        /// </summary>
        /// <param name="message">Incoming message</param>
        /// <param name="cryptoManager">Crypto manager</param>
        /// <returns>A transformed message (for further processing) or null</returns>
        IMercurioMessage Process(ICryptoManager cryptoManager, Serializer serializer, string userIdentity);

        /// <summary>
        /// Event fires when the message can be displayed (can happen at various points 
        /// throughout the processing pipeline: for example when encrypted, then again when
        /// decrypted)
        /// </summary>
        event MessageIsDisplayable MessageIsDisplayableEvent;
    }
}
