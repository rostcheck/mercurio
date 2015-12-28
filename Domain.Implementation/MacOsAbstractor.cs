using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MacOsAbstractor : IOSAbstractor
{
    public OSType GetOsType()
    {
        return OSType.Mac;
    }

    public char GetPathSeparatorChar()
    {
        return ':';
    }

    public string GetExecutableName(string bareName)
    {
        return bareName;
    }

	public string[] GetPaths()
	{
		var paths = new List<string>(Environment.ExpandEnvironmentVariables(Environment.GetEnvironmentVariable("PATH")).Split(GetPathSeparatorChar()));
		paths.Add("/opt/local/bin"); // Needed for debugging, Xamarin on Mac clears env vars
		return paths.ToArray();
	}
}