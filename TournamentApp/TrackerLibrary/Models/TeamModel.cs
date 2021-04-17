using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class TeamModel
    {
        
        public int Id { get; internal set; }
        /// <summary>
        /// Represent the team name in the tournament
        /// </summary>
        public string TeamName { get; set; }
        /// <summary>
        /// Represent the person model in the tournament tracker
        /// </summary>
        public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();
        
        
    }
}
