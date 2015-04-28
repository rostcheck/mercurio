using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class MercurioCommandBuilder
    {
        private List<IExecutableMercurioCommand> _availableCommands;

        public MercurioCommandBuilder(MercurioShellContext context)            
        {
            _availableCommands = new List<IExecutableMercurioCommand>();
            _availableCommands.Add(new ShowContainerCommand(context));
        }

        public IExecutableMercurioCommand BuildCommand(string command)
        {
            foreach (var availableCommand in _availableCommands)
            {
                if (availableCommand.RecognizeCommand(command))
                {
                    if (availableCommand.ValidateSyntax(command) == false)
                        throw new MercurioShellSyntaxException("invalid syntax");
                    else
                        return availableCommand;
                }
            }
            throw new MercurioShellException("Command not recognized");
        }
    }
}
