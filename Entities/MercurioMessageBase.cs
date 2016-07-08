using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercurio.Domain;

namespace Entities
{
    public class MercurioMessageBase
    {
        public event MessageIsDisplayable MessageIsDisplayableEvent;

        protected void RaiseMessageIsDisplayable(IMercurioMessage message)
        {
            if (MessageIsDisplayableEvent != null)
                MessageIsDisplayableEvent(message);
        }
    }
}
