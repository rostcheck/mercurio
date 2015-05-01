using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public interface IExecutableMercurioCommand
    {
        bool RecognizeCommand(string commandName);
        void ValidateSyntax(string commandName, Arguments args); // Can throw MercurioShellSyntaxException
        string ShowHelp();
        ICollection<string> ExecuteCommand(string command, Arguments args, MercurioShellContext context);
    }
}
