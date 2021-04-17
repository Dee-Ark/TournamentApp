using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class MatchupModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Represent the match up entries 
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();
        /// <summary>
        /// Represent the winner in the tournament
        /// </summary>
        public TeamModel Winner { get; set; }
        /// <summary>
        /// Represent the first rounal up in the competition
        /// </summary>
        public int MatchupRound { get; set; }

        public int WinnerId { get; set; }
        public string DisplayName
        {
            get
            {
                string output = "";

                foreach (MatchupEntryModel me in Entries)
                {
                    if (me.TeamCompeting != null)
                    {
                        if (output.Length == 0)
                        {
                            output = me.TeamCompeting.TeamName;
                        }
                        else
                        {
                            output += $" vs. { me.TeamCompeting.TeamName }";
                        }
                    }
                    else
                    {
                        output = "Match Not Yet Determined";
                        break;
                    }
                }

                return output;
            }
        }

    }
}
