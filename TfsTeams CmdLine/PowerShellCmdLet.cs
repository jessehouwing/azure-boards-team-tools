using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management.Automation;

namespace CommunityTfsTeamTools.TfsTeams.TfsTeams
{

    [Cmdlet(VerbsCommon.Add, "TfsTeamMember")]
    class TfsTeamsCmdLEt: TeamProjectCmdlet 
    {

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Team { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Member { get; set; }

    }


    public abstract class TeamProjectCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public Uri CollectionUri { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string TeamProject { get; set; }
    }
}
