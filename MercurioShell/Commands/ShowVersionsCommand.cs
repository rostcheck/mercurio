﻿using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell.Commands
{
    public class ShowVersionsCommand : CommandBase
    {
        public ShowVersionsCommand()
        {
            AddRequiredParameter("document-name", "name");
        }

        protected override ICollection<string> Execute(string command, Arguments arguments, MercurioShellContext context)
        {
            VerifyContainerIsOpen(context);

            var returnList = new List<string>();
            var versions = context.OpenContainer.ListAvailableVersions(arguments["document-name"]);
            if (versions == null)
                returnList.Add("Document " + arguments["document-name"] + " was not found in container " + context.OpenContainer.Name);
            else
            {
                returnList = new List<string>() { string.Format("Available versions for document {0} are:", arguments["document-name"]) };
                returnList.AddRange(versions.Select(s => s.ToString()));
            }
            return returnList;
        }
    }
}
