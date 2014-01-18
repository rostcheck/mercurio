using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace TestFunctionality
{
    /// <summary>
    /// Common interface of repositories that store messages
    /// </summary>
    public interface IMessageStore
    {
        void Store(IMercurioMessage message, string recipientAddress);
        List<IMercurioMessage> Retrieve(string recipient);
    }
}
