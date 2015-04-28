using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    /// <summary>
    /// Interprets a text command and dispatches appropriate behavior
    /// </summary>
    public class MercurioCommandShell
    {
        public MercurioCommandShell(string inputCommands = null)
        {

        }

        public object ExecuteCommand(string command)
        {
            //TODO: parse command

            //TODO: dispatch command
            return DispatchCommand();
        }

        public object DispatchCommand()
        {
            return null;
        }

        public void InstallCommand(string dllPath)
        {
            // Open the DLL, use reflection to find classes derived from PSCustomSnapIn, add them to cmdlet dispatch list
            throw new NotImplementedException();
        }
    }
}
