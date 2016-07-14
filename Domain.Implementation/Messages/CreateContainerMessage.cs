using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Mercurio.Domain;

namespace Mercurio.Domain.Implementation
{
	[Serializable]
	public class CreateContainerMessage : MercurioMessageBase, IMercurioMessage
	{
		private string _containerName;
		private string _cryptoManagerName;
		private string _revisionRetentionPolictyTypeName;
		private Guid _containerId;

		public CreateContainerMessage(string senderAddress, string recipientAddress, string containerName, string cryptoManagerName, string revisionRetentionPolicyTypeName, Guid containerId)
		{
			ValidateParameter("ContainerName", containerName);
			ValidateParameter("CryptoManagerName", cryptoManagerName);
			ValidateParameter("RevisionRetentionPolicyTypeName", revisionRetentionPolicyTypeName);
			ValidateParameter("ContainerId", containerId.ToString());

			var content = GetContent(containerName, cryptoManagerName, revisionRetentionPolicyTypeName, containerId);
			Initialize(senderAddress, recipientAddress, content);

			_containerName = containerName;
			_cryptoManagerName = cryptoManagerName;
			_revisionRetentionPolictyTypeName = revisionRetentionPolicyTypeName;
			_containerId = containerId;
		}

		private string GetContent(string containerName, string cryptoManagerName, string revisionRetentionPolicyTypeName, Guid containerId)
		{
			return containerName + ContentSeparator + cryptoManagerName + ContentSeparator + revisionRetentionPolicyTypeName + ContentSeparator + containerId.ToString();
		}

		private string ContainerName 
		{
			get { return _containerName; }
		}

		private string CryptoManagerName 
		{
			get { return _cryptoManagerName; }
		}

		private string RevisionRetentionPolictyTypeName 
		{
			get { return _revisionRetentionPolictyTypeName; }
		}

		private Guid ContainerId
		{
			get { return _containerId; }
		}

		public override string ToString()
		{
			return this._containerName;
		}

		public CreateContainerMessage(SerializationInfo info, StreamingContext context)
		{
			base.Deserialize(info, context);
			var fields = this.Content.Split(ContentSeparator.ToCharArray()[0]);
			if (fields.Length != 4)
				throw new MercurioException("CreateContainerMessage does not contain correct content");
			this._containerName = fields[0];
			this._cryptoManagerName = fields[1];
			this._revisionRetentionPolictyTypeName = fields[2];
			this._containerId = new Guid(fields[3]);
		}
	}
}

