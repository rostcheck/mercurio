using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    public class BasicStoragePlan : IStoragePlan
    {
        public string Name
        {
            get { return "BasicStoragePlan"; }
        }
    }
}
