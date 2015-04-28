using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class ShowContainerCommand : IExecutableMercurioCommand
    {
        private readonly MercurioShellContext _context;

        public ShowContainerCommand(MercurioShellContext context)
        {
            _context = context;
        }

        public bool RecognizeCommand(string command)
        {
            return (command.ToLower().Contains("show-containers"));
        }

        public bool ValidateSyntax(string command)
        {
            // Valid synax is show-containers w/ no args
            return (command.ToLower().Trim() == "show-containers");
        }

        public string ShowHelp()
        {
            return "Usage: Show-Containers";
        }

        public ICollection<string> ExecuteCommand(string command, MercurioShellContext context)
        {
            if (context == null || context.Environment == null)
                throw new ArgumentException("Invalid context passed to command");

            return context.Environment.GetContainers().Select(s => s.Name).ToList();
        }
    }
}
