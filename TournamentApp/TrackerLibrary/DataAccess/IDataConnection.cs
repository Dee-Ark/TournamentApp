using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAcces
{
    public interface IDataConnection
    {
        void  CreatePrize(PrizeModel model);
        void CreatePerson(PersonModel model);
        void Createteam(TeamModel model);
        void CreateTournament(TournamentModel model);
        void UpdateMatchUp(MatchupModel model);
        void CompleteTournament(TournamentModel model);
        List<TeamModel> GetTeams_All();
        List<PersonModel> GetAll_Person ();
        List<TournamentModel> GetTournament_All();
    }
}
