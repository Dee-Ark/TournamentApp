using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAcces
{
    public class SqlConnector : IDataConnection
    {
        private const string db = "Tournaments";
        public void CreatePerson(PersonModel model)
        {
            using(IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@FirstName", model.FirstName);
                p.Add("@LastName", model.LastName);
                p.Add("@EmailAddress", model.EmailAddress);
                p.Add("@CellPhoneNumber", model.CellPhoneNumber);
                p.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.People_InsertAllvalue", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@Id");
            }
        } 
        public void CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@PlaceNumber", model.PlaceNumber);
                p.Add("@PlaceName", model.PlaceName);
                p.Add("@PrizeAmount", model.PrizeAmount);
                p.Add("@PrizePercentage", model.PrizePercentage);
                p.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.Prizes_InsertAllvalue", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@Id");
            }
        }

        public void Createteam(TeamModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@TeamName", model.TeamName);
                p.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.Teams_InsertAllvalues", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@Id");

                foreach (PersonModel tmp in model.TeamMembers)
                {
                    var per = new DynamicParameters();
                    per.Add("@TeamId", model.Id);
                    per.Add("@PersonId", tmp.Id);
                    //per.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("dbo.TeamMembers_InsertAllValues", per, commandType: CommandType.StoredProcedure);

                    //model.Id = p.Get<int>("@Id");
                }
            }
        }

        public void CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                SaveTournament(connection, model);

                SaveTournamentPrizes(connection, model);

                SaveTournamentEntries(connection, model);

                SaveTournamentRounds(connection, model);
                
                TournamentLogic.UpdateTournamentResult(model);
            }
        }

        private void SaveTournament(IDbConnection connection, TournamentModel model)
        {
            var p = new DynamicParameters();
            p.Add("TournamentName", model.TournamentName);
            p.Add("EntryFee", model.EntryFee);
            p.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("dbo.Tournaments_InsertAllvalues", p, commandType: CommandType.StoredProcedure);

            model.Id = p.Get<int>("@Id");
        }

        private void SaveTournamentRounds(IDbConnection connection, TournamentModel model)
        {
            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    var p = new DynamicParameters();
                    p.Add("@TournamentId", model.Id);
                    p.Add("@MatchupRound", matchup.MatchupRound);
                    p.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("dbo.Matchups_InsertAllValues", p, commandType: CommandType.StoredProcedure);

                    matchup.Id = p.Get<int>("@Id");

                    foreach (MatchupEntryModel entry in matchup.Entries)
                    {
                        p = new DynamicParameters();
                        p.Add("@MatchupsId", matchup.Id);

                        if (entry.ParentMatch == null)
                        {
                            p.Add("@ParentMatchupId", null);
                        }
                        else
                        {
                            p.Add("@ParentMatchupId", entry.ParentMatch.Id);
                        }

                        if (entry.TeamCompeting == null)
                        {
                            p.Add("@TeamCompetingId", null);
                        }
                        else
                        {
                            p.Add("@TeamCompetingId", entry.TeamCompeting.Id);
                        }

                        p.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                        connection.Execute("dbo.MatchupEntries_InsertAllValues", p, commandType: CommandType.StoredProcedure);
                    }
                }
            }
        }
        private void SaveTournamentPrizes(IDbConnection connection, TournamentModel model)
        {
            foreach (PrizeModel pz in model.Prizes)
            {
                var per = new DynamicParameters();
                per.Add("@TournamentId", model.Id);
                per.Add("@PrizeId", pz.Id);
                per.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.TournamentPrizes_InsertAllValues", per, commandType: CommandType.StoredProcedure);
            }
        }

        private void SaveTournamentEntries(IDbConnection connection, TournamentModel model)
        {
            foreach (TeamModel tem in model.EnteredTeams)
            {
                var per = new DynamicParameters();
                per.Add("@TournamentId", model.Id);
                per.Add("@TeamId", tem.Id);
                per.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.TournamentEntries_InsertAllValues", per, commandType: CommandType.StoredProcedure);
            }
        }

        public List<PersonModel> GetAll_Person()
        {
            List<PersonModel> output;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
               output = connection.Query<PersonModel>("dbo.People_GetAll @FirstName = ''").ToList();
            }

            return output;
        }

        public List<TeamModel> GetTeams_All()
        {
            List<TeamModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TeamModel>("dbo.Team_GetAll @TournamentId = ''").ToList();

                foreach (TeamModel team in output)
                {
                    var p = new DynamicParameters();
                    p.Add("@TeamId", team.Id);
                    team.TeamMembers = connection.Query<PersonModel>("dbo.TeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                }
            }

            return output;
        }

        public List<TournamentModel> GetTournament_All()
        {
            List<TournamentModel> output;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TournamentModel>("dbo.Tournaments_GetAll").ToList();
                var p = new DynamicParameters();
                foreach (TournamentModel tm in output)
                {
                    p = new DynamicParameters();
                    p.Add("@TournamentId", tm.Id);
                    tm.Prizes = connection.Query<PrizeModel>("Prizes_GetByTournamentId", p, commandType: CommandType.StoredProcedure).ToList();

                    p = new DynamicParameters();
                    p.Add("@TournamentId", tm.Id);
                    tm.EnteredTeams = connection.Query<TeamModel>("dbo.Teams_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    foreach (TeamModel team in tm.EnteredTeams)
                    {
                        p = new DynamicParameters();
                        p.Add("@TeamId", team.Id);

                        team.TeamMembers = connection.Query<PersonModel>("dbo.TeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                    }
                         p = new DynamicParameters();
                         p.Add("@TournamentId", tm.Id);

                         List<MatchupModel> matchups = connection.Query<MatchupModel>("dbo.Matchups_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();
                    foreach (MatchupModel m in matchups)
                    {
                        p = new DynamicParameters();
                        p.Add("@MatchupsId", m.Id);

                        m.Entries = connection.Query<MatchupEntryModel>("dbo.MatchupEntries_GetByMatchup", p, commandType: CommandType.StoredProcedure).ToList();

                        List<TeamModel> allTeams = GetTeams_All();

                        if (m.WinnerId > 0)
                        {
                            m.Winner = allTeams.Where(x => x.Id == m.WinnerId).First();
                        }

                        foreach (var me in m.Entries)
                        {
                            if (me.TeamCompetingId > 0)
                            {
                                me.TeamCompeting = allTeams.Where(x => x.Id == me.TeamCompetingId).First();
                            }

                            if (me.parentMatchupId > 0)
                            {
                                me.ParentMatch = matchups.Where(x => x.Id == me.parentMatchupId).First();
                            }
                        }
                        List<MatchupModel> currentRow = new List<MatchupModel>();
                        int currentRound = 1;

                        foreach (MatchupModel ms in matchups)
                        {
                            if (ms.MatchupRound > currentRound)
                            {
                                tm.Rounds.Add(currentRow);
                                currentRow = new List<MatchupModel>();
                                currentRound += 1;
                            }
                            currentRow.Add(ms);
                        }
                        tm.Rounds.Add(currentRow);
                    }
                }
            }
            return output;
        }
        public void UpdateMatchUp(MatchupModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                if (model.Winner != null)
                {
                    p.Add("@Id", model.Id);
                    p.Add("@WinnerId", model.Winner.Id);
                    connection.Execute("dbo.Matchups_Update", p, commandType: CommandType.StoredProcedure); 
                }

                foreach (MatchupEntryModel mt in model.Entries)
                {
                    if (mt.TeamCompeting != null)
                    {
                        p = new DynamicParameters();
                        p.Add("@Id", mt.Id);
                        p.Add("@TeamCompetingId", mt.TeamCompeting.Id);
                        p.Add("@Score", mt.Score);
                        connection.Execute("dbo.MatchupEntries_Update", p, commandType: CommandType.StoredProcedure); 
                    }
                }
            }
        }

        public void CompleteTournament(TournamentModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@Id", model.Id);
                connection.Execute("dbo.Tournaments_Complete", p, commandType: CommandType.StoredProcedure);
            }
        }
    }
}