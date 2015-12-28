using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    public interface IOSAbstractor
    {
        OSType GetOsType();
        char GetPathSeparatorChar();
        string GetExecutableName(string bareName);
		string[] GetPaths();
    }
}
