using CommandLine.Utility;
using System;
using System.Collections.Generic;

namespace MercurioShell.Commands
{
    // This does the exact same thing as undeleting a document (UnDeleteDocument checks internally for a 
    // schema anyway), but says "database" in help text
    public class UndeleteDatabaseCommand : CommandBase
    {
        public UndeleteDatabaseCommand()
        {
            AddRequiredParameter("database-name", "name");
            AddAlias("undeldb");
        }

        protected override ICollection<string> Execute(string commandName, Arguments arguments, MercurioShellContext context)
        {
            VerifyContainerIsOpen(context);
            string databaseName = arguments["database-name"];

            try
            {
                context.OpenContainer.UnDeleteDocument(databaseName, context.Environment.GetActiveIdentity());
                return new List<string>() { string.Format("Database {0} was undeleted", arguments["database-name"]) };
            }
            catch (Exception ex)
            {
                return new List<string>() { ex.Message };
            }
        }
    }
}
