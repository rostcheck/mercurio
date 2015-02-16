using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    public interface IDocument
    {
        string Name { get; }
        string Content { get; }
    }
}
