using System;
using CommandLine.Utility;
using System.Collections.Generic;

namespace MercurioShell
{
    public class DeleteDatabaseCommand : CommandBase, IExecutableMercurioCommand
    {
        public DeleteDatabaseCommand()
        {
            AddRequiredParameter("database-name", "name");
            AddOptionalParameter("hard-delete", "hard-delete");
            AddAlias("deldb");
            AddAlias("rmdb");
        }

        protected override ICollection<string> Execute(string commandName, Arguments arguments, MercurioShellContext context)
        {
            VerifyContainerIsOpen(context);

            if (arguments.Contains("hard-delete"))
            {
                if (context.ConfirmAction("WARNING: Deleting a database will delete all its contents forever. Are you sure you want to do this?", context.Environment))
                {
                    if (arguments.Contains("hard-delete"))
                    {
                        context.OpenContainer.DeleteDocumentHard(arguments["database-name"], context.Environment.GetActiveIdentity());
                    }
                }
                else
                    return new List<string>() { "Passphrase not correct - document not deleted" };
            }
            else
            {
                context.OpenContainer.DeleteDocumentSoft(arguments["database-name"], context.Environment.GetActiveIdentity());
            }
            return new List<string>() { string.Format("Database {0} was deleted", arguments["database-name"]) };
        }
    }
}
