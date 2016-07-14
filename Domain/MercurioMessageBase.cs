using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Mercurio.Domain
{
	public class MercurioMessageBase : IMercurioMessage
    {
		public MercurioMessageBase()
		{
		}

		public MercurioMessageBase(string senderAddress, string recipientAddress, string content, Guid contentId)
		{
			_senderAddress = senderAddress;
			_recipientAddress = recipientAddress;
			_contentId = contentId;	
			_content = content;		
		}

		#region IMercurioMessage implementation
		protected const string SenderAddressName = "sender_address";
		protected const string RecipientAddressName = "recipient_address";
		protected const string ContentName = "content";
		protected const string ContentIDName = "content_id";

		private Guid _contentId;
		private string _senderAddress;
		private string _recipientAddress;
		private string _content;

		public virtual Guid ContentID
		{
			get
			{
				return _contentId;
			}
		}

		public virtual string SenderAddress
		{
			get
			{
				return _senderAddress;
			}
		}

		public virtual string RecipientAddress
		{
			get
			{
				return _recipientAddress;
			}
		}

		public virtual string Content
		{
			get
			{
				return _content;
			}
			protected set 
			{
				_content = value;
			}
		}

		public virtual bool Encryptable
		{
			get
			{
				return true;
			}
		}

		public event MessageIsDisplayable MessageIsDisplayableEvent;

		protected void RaiseMessageIsDisplayable(IMercurioMessage message)
		{
			if (MessageIsDisplayableEvent != null)
				MessageIsDisplayableEvent(message);
		}

		public virtual IMercurioMessage Process(ICryptoManager cryptoManager, Serializer serializer, string userIdentity)
		{
			RaiseMessageIsDisplayable(this);
			return null;
		}
		#endregion

		protected virtual void Initialize(string senderAddress, string recipientAddress, string content)
		{
			Initialize(senderAddress, recipientAddress, content, Guid.NewGuid());
		}

		protected virtual void Initialize(string senderAddress, string recipientAddress, string content, Guid contentId)
		{
			ValidateParameter("SenderAddress", senderAddress);
			ValidateParameter("ReceipientAddress", recipientAddress);
			if (content == null || content == string.Empty)
				throw new ArgumentException("Cannot initialize message without content");

			this._senderAddress = senderAddress;
			this._recipientAddress = recipientAddress;
			this._contentId = contentId;
			this._content = content;
		}

		#region Serialization
		public virtual void Deserialize(SerializationInfo info, StreamingContext ctxt)
		{
			this._senderAddress = info.GetString(SenderAddressName);
			this._recipientAddress = info.GetString(RecipientAddressName);
			this._contentId = (Guid)info.GetValue(ContentIDName, typeof(Guid));
			this._content = info.GetString(ContentName);
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(RecipientAddressName, _recipientAddress);
			info.AddValue(SenderAddressName, _senderAddress);
			info.AddValue(ContentIDName, _contentId);
			info.AddValue(ContentName, _content);
		}
		#endregion

		public const string ContentSeparator = ":";
		public const string ContentSubSeparator = "|";

		protected void ValidateParameter(string parameterName, string parameterValue)
		{
			if (parameterValue == null || parameterValue == string.Empty)
				throw new ArgumentException("Cannot initialize message without " + parameterName);
			if (parameterValue.Contains(ContentSeparator))
				throw new ArgumentException(parameterName + " cannot contain " + ContentSeparator);
			if (parameterValue.Contains(ContentSubSeparator))
				throw new ArgumentException(parameterName + " cannot contain " + ContentSubSeparator);
		}

		public override string ToString()
		{
			return this.Content;
		}
    }
}
