using Mercurio.Domain;
using Mercurio.Domain.Implementation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    public static class OSAbstractorFactory
    {
        public static IOSAbstractor GetOsAbstractor()
        {
            switch(GetOsType())
            {
                case OSType.Windows:
                    return new WindowsOsAbstractor();
                case OSType.Linux:
                    return new LinuxOsAbstractor();
                case OSType.Mac:
                    return new MacOsAbstractor();
                default:
                    throw new MercurioException("System OS type cannot be recognized");
            }
        }

        public static OSType GetOsType()
        {
            if (Path.DirectorySeparatorChar == '\\')
                return OSType.Windows;
            var unameOutput = ReadProcessOutput("uname");
            if (unameOutput.Contains("Darwin"))
                return OSType.Mac;
            else if (unameOutput.Contains("Linux"))
                return OSType.Linux;
            else return OSType.Unknown;
        }

        private static string ReadProcessOutput(string name)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = name;
                p.Start();

                // Read the output stream first and then wait.
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                if (output == null) output = "";
                output = output.Trim();
                return output;
            }
            catch
            {
                return "";
            }
        }
    }
}
