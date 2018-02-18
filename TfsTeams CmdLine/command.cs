// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
// This is sample code only, do not use in production environments
namespace CommunityTfsTeamTools.TfsTeams.TfsTeams
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CommandBase
    {
        protected string[] expectedArguments;
        protected string[] optionalArguments;
        private Dictionary<string, string> arguments;

        public bool ParseArguments(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            bool r = true;
            Dictionary<string, string> d = new Dictionary<string, string>();
            foreach (string s in args)
            {
                if (s.Contains(":"))
                {
                    string command = s.Substring(1, s.IndexOf(':') - 1);
                    string value = s.Substring(s.IndexOf(":", StringComparison.OrdinalIgnoreCase) + 1);
                    d.Add(command.ToUpper(), value);
                }
                else
                {
                    Console.WriteLine("Unexpected argument: " + s);
                    r = false;
                }
            }

            this.arguments = d;
            return r;
        }

        public bool Validate()
        {
            bool r = true;
            List<string> argLst = new List<string>();
            argLst.AddRange(this.arguments.Keys);

            foreach (string expected in this.expectedArguments)
            {
                if (!this.arguments.Keys.Contains(expected.ToUpper()))
                {
                    Console.WriteLine("Error: Expected argument " + expected);
                    r = false;
                }
                else
                {
                    argLst.Remove(expected.ToUpper());
                }
            }

            if (this.optionalArguments != null)
            {
                foreach (string optional in this.optionalArguments)
                {
                    if (this.arguments.Keys.Contains(optional.ToUpper()))
                    {
                        argLst.Remove(optional.ToUpper());
                    }
                }
            }

            if (argLst.Count > 0)
            {
                r = false;
                foreach (string s in argLst)
                {
                    Console.WriteLine("Unexpected argument : " + s);
                }
            }

            return r;
        }

        public virtual void Run()
        {
        }

        public string CommandSyntax()
        {

            string s = string.Empty;
            foreach (string expected in this.expectedArguments)
            {
                s += @"/" + expected + ": ";
            }

            if (this.optionalArguments != null)
            {
                foreach (string optional in this.optionalArguments)
                {
                    s += @"[/" + optional + ": ]";

                }
            }

            return s;
        }

        protected string GetArgument(string argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException("argument");
            }

            if (this.arguments.Keys.Contains(argument.ToUpper()))
            {
                return this.arguments[argument.ToUpper()];
            }

            return null;
        }
        public string CommandName { get; set; }
    }

    public class ListTeamCommand : CommandBase
    {
        public ListTeamCommand()
        {
            this.CommandName = "ListTeams";
            this.expectedArguments = new[] { "Collection", "TeamProject" };
        }

        public override void Run()
        {
            using (TeamWrapper team = new TeamWrapper(new Uri(GetArgument("Collection")), GetArgument("TeamProject")))
            {
                foreach (string s in team.ListTeams())
                {
                    Console.WriteLine(s);
                }
            }
        }
    }

    public class CreateTeamCommand : CommandBase
    {
        public CreateTeamCommand()
        {
            this.CommandName = "CreateTeam";
            this.expectedArguments = new[] { "Collection", "TeamProject", "Team" };
            this.optionalArguments = new[] { "Description" };
        }

        public override void Run()
        {
            using (TeamWrapper team = new TeamWrapper(new Uri(this.GetArgument("Collection")), GetArgument("TeamProject")))
            {
                team.CreateTeam(this.GetArgument("Team"), this.GetArgument("Description"));
            }
        }
    }

    public class DeleteTeamCommand : CommandBase
    {
        public DeleteTeamCommand()
        {
            this.CommandName = "DeleteTeam";
            this.expectedArguments = new[] { "Collection", "TeamProject", "Team" };
            
        }

        public override void Run()
        {
            using (TeamWrapper team = new TeamWrapper(new Uri(this.GetArgument("Collection")), GetArgument("TeamProject")))
            {
                string msg;
                if(!team.DeleteTeam(this.GetArgument("Team"), out msg))
                {
                    Console.WriteLine(msg);
                }

            }
        }
    }

    public class GetDefaultTeamCommand : CommandBase
    {
        public GetDefaultTeamCommand()
        {
            this.CommandName = "GetDefaultTeam";
            this.expectedArguments = new[] { "Collection", "TeamProject" };

        }

        public override void Run()
        {
            using (TeamWrapper team = new TeamWrapper(new Uri(this.GetArgument("Collection")), GetArgument("TeamProject")))
            {
                string msg;
                string defTeam = team.GetDefaultTeam(out msg);
                if (defTeam!=null)
                {
                       Console.WriteLine(defTeam);
                }
                else
                {
                    Console.WriteLine(msg);
                }

            }
        }
    }

    public class SetDefaultTeamCommand : CommandBase
    {
        public SetDefaultTeamCommand()
        {
            this.CommandName = "SetDefaultTeam";
            this.expectedArguments = new[] { "Collection", "TeamProject", "Team" };

        }

        public override void Run()
        {
            using (TeamWrapper team = new TeamWrapper(new Uri(this.GetArgument("Collection")), GetArgument("TeamProject")))
            {
                string msg;
                if (!team.SetDefaultTeam(this.GetArgument("Team"), out msg))
                {
                    Console.WriteLine(msg);
                }

            }
        }
    }


    public class RenameTeamCommand : CommandBase
    {
        public RenameTeamCommand()
        {
            this.CommandName = "RenameTeam";
            this.expectedArguments = new[] { "Collection", "TeamProject", "Team", "NewTeamName" };
            this.optionalArguments = new[] { "NewDescription" };

        }

        public override void Run()
        {
            using (TeamWrapper team = new TeamWrapper(new Uri(this.GetArgument("Collection")), GetArgument("TeamProject")))
            {
                string msg;
                if (!team.RenameTeam(this.GetArgument("Team"),  this.GetArgument("NewTeamName"), this.GetArgument("NewDescription"), out msg))
                {
                    Console.WriteLine(msg);
                }

            }
        }
    }

    public class ListTeamMembersCommand : CommandBase
    {
        public ListTeamMembersCommand()
        {
            this.CommandName = "ListTeamMembers";
            this.expectedArguments = new[] { "Collection", "TeamProject", "Team" };

        }

        public override void Run()
        {
            using (TeamWrapper team = new TeamWrapper(new Uri(this.GetArgument("Collection")), GetArgument("TeamProject")))
            {
                string msg;
                List<string> teamMembers = team.ListMembers(this.GetArgument("Team"), out msg);
                if (teamMembers == null)
                {
                    Console.WriteLine(msg);
                }
                else
                {
                    foreach(string s in teamMembers )
                    {
                        Console.WriteLine(s);
                    }
                }
            }
        }
    }
    public class AddUserCommand : CommandBase
    {
        public AddUserCommand()
        {
            this.CommandName = "AddUser";
            this.expectedArguments = new[] { "Collection", "TeamProject", "Team", "User" };
        }

        public override void Run()
        {
            using (TeamWrapper team = new TeamWrapper(new Uri(this.GetArgument("Collection")), GetArgument("TeamProject")))
            {
                string msg;
                if (!team.AddMember(this.GetArgument("Team"), this.GetArgument("User"), out msg))
                {
                    Console.WriteLine(msg);
                }
            }
        }
    }

    public class RemoveUserCommand : CommandBase
    {
        public RemoveUserCommand()
        {
            this.CommandName = "RemoveUser";
            this.expectedArguments = new[] { "Collection", "TeamProject", "Team", "User" };
        }

        public override void Run()
        {
            using (TeamWrapper team = new TeamWrapper(new Uri(this.GetArgument("Collection")), GetArgument("TeamProject")))
            {
                string msg;
                if (!team.RemoveMember(this.GetArgument("Team"), this.GetArgument("User"), out msg))
                {
                    Console.WriteLine(msg);
                }
            }
        }
    }

    public class AddTeamAdminCommand : CommandBase
    {
        public AddTeamAdminCommand()
        {
            this.CommandName = "AddTeamAdministrator";
            this.expectedArguments = new[] { "Collection", "TeamProject", "Team", "User" };
        }

        public override void Run()
        {
            using (TeamWrapper team = new TeamWrapper(new Uri(this.GetArgument("Collection")), GetArgument("TeamProject")))
            {
                string msg;
                if (!team.AddTeamAdministrator(this.GetArgument("Team"), this.GetArgument("User"), out msg))
                {
                    Console.WriteLine(msg);
                }
            }
        }
    }

    public class RemoveTeamAdminCommand : CommandBase
    {
        public RemoveTeamAdminCommand()
        {
            this.CommandName = "RemoveTeamAdministrator";
            this.expectedArguments = new[] { "Collection", "TeamProject", "Team", "User" };
        }

        public override void Run()
        {
            using (TeamWrapper team = new TeamWrapper(new Uri(this.GetArgument("Collection")), GetArgument("TeamProject")))
            {
                string msg;
                if (!team.RemoveTeamAdministrator(this.GetArgument("Team"), this.GetArgument("User"), out msg))
                {
                    Console.WriteLine(msg);
                }
            }
        }
    }

    public class ListTeamAdminCommand : CommandBase
    {
        public ListTeamAdminCommand()
        {
            this.CommandName = "ListTeamAdministrators";
            this.expectedArguments = new[] { "Collection", "TeamProject", "Team" };
        }

        public override void Run()
        {
            using (TeamWrapper team =
                new TeamWrapper(new Uri(this.GetArgument("Collection")), GetArgument("TeamProject")))
            {
                string msg;
                List<string> teamMembers = team.ListTeamAdministrators(this.GetArgument("Team"), out msg);
                if (teamMembers == null)
                {
                    Console.WriteLine(msg);
                }
                else
                {
                    foreach (string s in teamMembers)
                    {
                        Console.WriteLine(s);
                    }
                }
            }
        }
    }

    public class ShowUsageCommand : CommandBase
    {
        public ShowUsageCommand()
        {
            this.CommandName = "?";
            this.expectedArguments = new string[] { };
        }

        public override void Run()
        {
            Console.WriteLine(@"------------------------------------------------------------------------");
            Console.WriteLine(@"TfsTeams Command line utility  - (c) Community TFS Team Tools ");
            Console.WriteLine(@"------------------------------------------------------------------------");
            Console.WriteLine(@"Usage:");
            Console.WriteLine(@"       TfsTeams ListTeams /collection:<collectionurl> /teamproject:<teamprojectname>");
            Console.WriteLine("");
            Console.WriteLine(@"       TfsTeams CreateTeam /Team:<teamname> [/Description:<description>] /collection:<collectionurl> /teamproject:<teamprojectname>");
            Console.WriteLine(@"       TfsTeams DeleteTeam /Team:<teamname> /collection:<collectionurl> /teamproject:<teamprojectname>");
            Console.WriteLine(@"       TfsTeams RenameTeam /Team:<teamname> /NewTeamName:<newteamname> [/NewDescription:<description>] /collection:<collectionurl> /teamproject:<teamprojectname>");
            Console.WriteLine("");
            Console.WriteLine(@"       TfsTeams GetDefaultTeam /collection:<collectionurl> /teamproject:<teamprojectname>");
            Console.WriteLine(@"       TfsTeams SetDefaultTeam /Team:<teamname>  /collection:<collectionurl> /teamproject:<teamprojectname>");
            Console.WriteLine("");
            Console.WriteLine(@"       TfsTeams ListTeamMembers  /Team:<teamname> /collection:<collectionurl> /teamproject:<teamprojectname>");
            Console.WriteLine("");
            Console.WriteLine(@"       TfsTeams AddUser /User:<domain\user> /Team:<teamname> /collection:<collectionurl> /teamproject:<teamprojectname>");
            Console.WriteLine(@"       TfsTeams RemoveUser /User:<domain\user> /Team:<teamname> /collection:<collectionurl> /teamproject:<teamprojectname>");
            Console.WriteLine("");
            Console.WriteLine(@"       TfsTeams ListTeamAdministrators  /Team:<teamname> /collection:<collectionurl> /teamproject:<teamprojectname>");
            Console.WriteLine("");
            Console.WriteLine(@"       TfsTeams AddTeamAdministrator /User:<domain\user> /Team:<teamname> /collection:<collectionurl> /teamproject:<teamprojectname>");
            Console.WriteLine(@"       TfsTeams RemoveTeamAdministrator /User:<domain\user> /Team:<teamname> /collection:<collectionurl> /teamproject:<teamprojectname>");

            Console.WriteLine(string.Empty);
        }
    }
}
