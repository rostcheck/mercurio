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
        string Name { get; }
        string RewriteBeforeParsing(string commandString);
        bool RecognizeCommand(string commandName);
		string RecognizePartialCommand(string commandName);
        void ValidateSyntax(string commandName, Arguments args); // Can throw MercurioShellSyntaxException
        string ShowHelp();
		ICollection<string> RecognizePartialArgument(string argumentName);
        ICollection<string> ExecuteCommand(string command, Arguments args, MercurioShellContext context);
    }
}
