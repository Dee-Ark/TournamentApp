using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class MatchupEntryModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Represent one of the Team in the Matchup
        /// </summary>
        public TeamModel TeamCompeting { get; set; }
        /// <summary>
        /// Represent the score of the this particular Team
        /// </summary>
        public double Score { get; set; }

        public int TeamCompetingId { get; set; }

        public int ParentMatchupId { get; set; }

        public int parentMatchupId { get; set; }
        /// <summary>
        /// Represent the matchup that this team came from as the winner
        /// </summary>
        public MatchupModel ParentMatch { get; set; }
        
    }
}
