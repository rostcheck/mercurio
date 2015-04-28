using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public interface IExecutableMercurioCommand
    {
        bool RecognizeCommand(string command);
        bool ValidateSyntax(string command);
        string ShowHelp();
        ICollection<string> ExecuteCommand(string command, MercurioShellContext context);
    }
}
