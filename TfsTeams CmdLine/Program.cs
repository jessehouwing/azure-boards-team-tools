// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
// This is sample code only, do not use in production environments
namespace CommunityTfsTeamTools.TfsTeams.TfsTeams
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Program
    {
        
        public static void Main(string[] args)
        {

            CommandBase[] cmdList = new CommandBase[] {  new ShowUsageCommand(), new ListTeamCommand(), new CreateTeamCommand(), new DeleteTeamCommand(), new RenameTeamCommand(), new GetDefaultTeamCommand(), new SetDefaultTeamCommand(), new AddUserCommand(), new RemoveUserCommand(), new ListTeamMembersCommand(), new ListTeamAdminCommand(), new AddTeamAdminCommand(), new RemoveTeamAdminCommand() };

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (!args.Any())
            {
                ShowUsageCommand cmd = new ShowUsageCommand();
                cmd.Run();
            }
            else if(args[0] ==@"/?"   )
            {
                ShowUsageCommand cmd = new ShowUsageCommand();
                cmd.Run();
            }
            else
            {
                bool cmdFound = false;
                foreach (CommandBase cmd in cmdList)
                {
                    if (args[0].ToUpper() == cmd.CommandName.ToUpper())
                    {
                        cmdFound = true;
                        List<string> lstArg = new List<string>();
                        lstArg.AddRange(args);
                        lstArg.RemoveAt(0);

                        if (cmd.ParseArguments(lstArg.ToArray()))
                        {
                            if (cmd.Validate())
                            {
                                cmd.Run();
                            }
                        }
                    }
                }
                if (cmdFound != true)
                {
                    Console.WriteLine("Unknown command: " + args[0]  );
                    Console.WriteLine("Try running TfsTeams /?");
                }
            }
        }
    }
}
