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
}