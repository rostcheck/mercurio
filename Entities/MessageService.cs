using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class MessageService
    {
        public void Send(IMercurioMessage message)
        {
            queue.Add(message);
        }

        private IPersistentQueue queue;

        public MessageService(IPersistentQueue queue)
        {
            this.queue = queue;
        }
       
    }
}
