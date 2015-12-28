using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    public class WindowsOsAbstractor : IOSAbstractor
    {
        public OSType GetOsType()
        {
            return OSType.Windows;
        }

        public char GetPathSeparatorChar()
        {
            return ';';
        }

        public string GetExecutableName(string bareName)
        {
            return bareName + ".exe";
        }

		public string[] GetPaths()
		{
			return Environment.ExpandEnvironmentVariables(Environment.GetEnvironmentVariable("PATH")).Split(GetPathSeparatorChar());
		}
    }
}
