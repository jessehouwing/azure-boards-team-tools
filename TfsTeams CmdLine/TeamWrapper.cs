// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
// This is sample code only, do not use in production environments
namespace CommunityTfsTeamTools.TfsTeams.TfsTeams
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Framework.Client;
    using Microsoft.TeamFoundation.Framework.Common;
    using Microsoft.TeamFoundation.Server;

    internal class TeamWrapper : IDisposable
    {
        private readonly TfsTeamProjectCollection teamProjectCollection;
        private readonly TfsTeamService teamService;
        private readonly ProjectInfo projectInfo;
        private readonly IIdentityManagementService identityManagementService;

        public TeamWrapper(Uri collectionUri, string teamProjectName)
        {
            this.teamProjectCollection = new TfsTeamProjectCollection(collectionUri);
            this.teamService = this.teamProjectCollection.GetService<TfsTeamService>();
            this.identityManagementService = this.teamProjectCollection.GetService<IIdentityManagementService>();
            ICommonStructureService4 cssService = this.teamProjectCollection.GetService<ICommonStructureService4>();
            this.projectInfo = cssService.GetProjectFromName(teamProjectName);
        }

        public void Dispose()
        {
            this.teamProjectCollection.Dispose();
        }

        public List<string> ListTeams()
        {
            var teams = this.teamService.QueryTeams(this.projectInfo.Uri);
            return (from t in teams select t.Name).ToList();
        }

        public List<string> ListTeamMembers(string team, out string message)
        {
            List<string> lst = null;
            message = "";
            TeamFoundationTeam t = this.teamService.ReadTeam(this.projectInfo.Uri, team, null);
            if (t == null)
            {
                message = "Team [" + team + "] not found";
            }
            else
            {
                lst = new List<string>();
                foreach (TeamFoundationIdentity i in t.GetMembers(this.teamProjectCollection, MembershipQuery.Expanded))
                {
                    lst.Add(i.DisplayName);
                }
            }
            return lst;


        }

        public string GetDefaultTeam( out string message)
        {
            message = string.Empty;
            string defaultTeamName=null;

            TeamFoundationTeam t = this.teamService.GetDefaultTeam(this.projectInfo.Uri,  null);
            if (t == null)
            {
                message = "No default team found ";
                
            }
            else
            {
                defaultTeamName= t.Name;
            }


            return defaultTeamName;
        }


        public bool SetDefaultTeam(string team, out string message)
        {
            message = string.Empty;
            bool ret = true;

            TeamFoundationTeam t = this.teamService.ReadTeam(this.projectInfo.Uri, team, null);
            if (t == null)
            {
                message = "Team [" + team + "] not found";
                ret = false;
            }

          

            if (ret)
            {
                this.teamService.SetDefaultTeam(t);
                message = "Team [" + team + "] set to default team ";
            }



            return ret;
        }

        public Guid CreateTeam(string team, string description)
        {
            TeamFoundationTeam t = this.teamService.CreateTeam(this.projectInfo.Uri, team, description, null);
            return t.Identity.TeamFoundationId;
        }



        public bool RenameTeam(string team, string newTeamName, string newDescription, out string message)
        {
            message = string.Empty;
            bool ret = true;

            TeamFoundationTeam t = this.teamService.ReadTeam(this.projectInfo.Uri, team, null);
            if (t == null)
            {
                message = "Team [" + team + "] not found";
                ret = false;
            }

            if (ret)
            {
                // Each call below will cause a separate request to TFS
                this.identityManagementService.UpdateApplicationGroup(t.Identity.Descriptor, GroupProperty.Name, newTeamName);
                if (newDescription != null)
                {
                    this.identityManagementService.UpdateApplicationGroup(t.Identity.Descriptor, GroupProperty.Description, newDescription);
                } 

                message = "Team renamed ";
            }



            return ret;
        }
        public bool DeleteTeam(string team, out string message)
        {
            message = string.Empty;
            bool ret = true;

            TeamFoundationTeam t = this.teamService.ReadTeam (this.projectInfo.Uri, team, null);
            if (t == null)
            {
                message = "Team [" + team + "] not found";
                ret = false;
            }

            if (ret)
            {
                this.identityManagementService.DeleteApplicationGroup(t.Identity.Descriptor); 
                message = "Team deleted ";
            }

            

            return ret;
        }
        public bool AddUser(string team, string user, out string message)
        {
            message = string.Empty;
            bool ret = true;
            TeamFoundationTeam t = this.teamService.ReadTeam(this.projectInfo.Uri, team, null);
            TeamFoundationIdentity i = this.identityManagementService.ReadIdentity(IdentitySearchFactor.AccountName, user, MembershipQuery.Direct, ReadIdentityOptions.None);

            if (t == null)
            {
                message = "Team [" + team + "] not found";
                ret = false;
            }

            if (i == null)
            {
                message = "User [" + user + "] not found";
                ret = false;
            }

            if (ret)
            {
                this.identityManagementService.AddMemberToApplicationGroup(t.Identity.Descriptor, i.Descriptor);
                message = "User added ";
            }

            return ret;
        }

        public bool RemoveUser(string team, string user, out string message)
        {
            message = string.Empty;
            bool ret = true;
            TeamFoundationTeam t = this.teamService.ReadTeam(this.projectInfo.Uri, team, null);
            TeamFoundationIdentity i = this.identityManagementService.ReadIdentity(IdentitySearchFactor.AccountName, user, MembershipQuery.Direct, ReadIdentityOptions.None);

            if (t == null)
            {
                message = "Team [" + team + "] not found";
                ret = false;
            }

            if (i == null)
            {
                message = "User [" + user + "] not found";
                ret = false;
            }

            if (ret)
            {
                this.identityManagementService.RemoveMemberFromApplicationGroup( t.Identity.Descriptor, i.Descriptor);
                message = "User removed ";
            }

            return ret;
        }
  
    }
}
