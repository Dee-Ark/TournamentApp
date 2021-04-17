using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class TournamentModel
    {
        public event EventHandler<DateTime> OnTournamentComplete;
        public int Id { get; set; }
        /// <summary>Represent the tournament name for each team</summary>
        public string  TournamentName { get; set; }
        /// <summary>Represent the entry fee for each team in the competition</summary>
        public decimal EntryFee { get; set; }
        /// <summary>Represent the number of each team members entered by each teams.</summary>
        public List<TeamModel> EnteredTeams { get; set; } = new List<TeamModel>();
        /// <summary>Represent the prize model class</summary>
        public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();
        /// <summary>Represent the match up class</summary>
        public List<List<MatchupModel>> Rounds { get; set; } = new List<List<MatchupModel>>();
        public void CompleteTournament()
        {
            OnTournamentComplete?.Invoke(this, DateTime.Now);
        }
    }
}