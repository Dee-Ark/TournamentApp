using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

//*Load the Text File
//*Convert the Text to List<PrizeModel>
//Find the max ID
//Add the new record with the new ID (max + 1)
//Convert the Prizes to List<string>
//Save the List<string> to the text File

namespace TrackerLibrary.DataAccess.TextHelper
{
    public static class TextConnectorProcessor
    {
        public static string FullFilePath(this string fileName)
        {
            return $"{ ConfigurationManager.AppSettings["filePath"]}\\{fileName}";
        }

        public static List<string> LoadFile(this string file)
        {
            if (!File.Exists(file))
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }

        public static List<PrizeModel> ConverttoprizsModel(this List<string> Lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach (string line in Lines)
            {
                string[] cols = line.Split(',');

                PrizeModel p = new PrizeModel();
                p.Id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2];
                p.PrizeAmount = decimal.Parse(cols[3]);
                p.PrizePercentage = double.Parse(cols[4]);
                output.Add(p);
            }

            return output;
        }

        public static List<PersonModel> ConvertToPersonModel(this List<string> Lines)
        {
            List<PersonModel> output = new List<PersonModel>();
            foreach (string line in Lines)
            {
                string[] col = line.Split(',');

                PersonModel pers = new PersonModel();
                pers.Id = int.Parse(col[0]);
                pers.FirstName = col[1];
                pers.LastName = col[2];
                pers.EmailAddress = col[3];
                pers.CellPhoneNumber = col[4];
                output.Add(pers);
            }

            return output;
        }

        public static List<TeamModel> ConvertToTeamModel(this List<string> Lines)
        {
            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModel();
            foreach (string line in Lines)
            {
                string[] col = line.Split(',');

                TeamModel t = new TeamModel();
                t.Id = int.Parse(col[0]);
                t.TeamName = col[1];

                string[] personIds = col[2].Split('|');

                foreach (string Id in personIds)
                {
                    t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(Id)).First());
                }
                output.Add(t);
            }

            return output;
        }

        public static List<TournamentModel> ConvertToTournamentModel(this List<string> Lines)
        {
            List<TournamentModel> output = new List<TournamentModel>();
            List<TeamModel> teams = GlobalConfig.TeamFile.FullFilePath().LoadFile().ConvertToTeamModel();
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConverttoprizsModel();
            List<MatchupModel> matchups = GlobalConfig.MatchUpFile.FullFilePath().LoadFile().ConvertToMatchaUpModels();

            foreach (string line in Lines)
            {
                string[] col = line.Split(',');

                TournamentModel tm = new TournamentModel();
                tm.Id = int.Parse(col[0]);
                tm.TournamentName = col[1];
                tm.EntryFee = decimal.Parse(col[2]);

                string[] teamIds = col[3].Split('|');
                foreach (string id in teamIds)
                {
                    tm.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());
                }

                if (col[4].Length > 0)
                {
                    string[] prizeIds = col[4].Split('|');
                    foreach (string Id in prizeIds)
                    {
                        tm.Prizes.Add(prizes.Where(x => x.Id == int.Parse(Id)).First());
                    }
                }

                string[] rounds = col[5].Split('|');
                foreach (string round in rounds)
                {
                    string[] muText = round.Split('^');
                    List<MatchupModel> mu = new List<MatchupModel>();

                    foreach (string matchupModelTextId in muText)
                    {
                        mu.Add(matchups.Where(x => x.Id == int.Parse(matchupModelTextId)).First());
                    }

                    tm.Rounds.Add(mu);
                }
                output.Add(tm);
            }

            return output;
        }
        public static void SaveRoundsToFile(this TournamentModel model)
        {
            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    matchup.SaveMatchUpToFile();
                }
            }
        }
        public static void SaveMatchUpToFile(this MatchupModel matchup)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchUpFile.FullFilePath().LoadFile().ConvertToMatchaUpModels();

            int currentId = 1;

            if (matchups.Count > 0)
            {
                currentId = matchups.OrderByDescending(x => x.Id).First().Id + 1;
            }

            matchup.Id = currentId;

            matchups.Add(matchup);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.SaveEntryToFile();
            }

            List<string> lines = new List<string>();

            foreach (MatchupModel m in matchups)
            {
                string Winner = "";
                if (m.Winner != null)
                {
                    Winner = m.Winner.Id.ToString();
                }

                lines.Add($"{ m.Id },{ ConvertMatchupEntriesToString(m.Entries) },{ Winner },{ m.MatchupRound }");
            }

            File.WriteAllLines(GlobalConfig.MatchUpFile.FullFilePath(), lines);
        }

        public static void UpdateMatchToFile(this MatchupModel matchup)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchUpFile.FullFilePath().LoadFile().ConvertToMatchaUpModels();

            MatchupModel oldMatchup = new MatchupModel();
            foreach (MatchupModel m in matchups)
            {
                if (m.Id == matchup.Id)
                {
                    oldMatchup = m;
                }
            }
            matchups.Remove(oldMatchup);

            matchups.Add(matchup);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.UpdateEntryToFile();
            }

            List<string> lines = new List<string>();

            foreach (MatchupModel m in matchups)
            {
                string Winner = "";
                if (m.Winner != null)
                {
                    Winner = m.Winner.Id.ToString();
                }

                lines.Add($"{ m.Id },{ ConvertMatchupEntriesToString(m.Entries) },{ Winner },{ m.MatchupRound }");
            }

            File.WriteAllLines(GlobalConfig.MatchUpFile.FullFilePath(), lines);
        }

        public static void UpdateEntryToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchUpEntryFile.FullFilePath().LoadFile().ConvertToMatchUpEntryModels();
            MatchupEntryModel oldEntry = new MatchupEntryModel();

            foreach (MatchupEntryModel mt in entries)
            {
                if (mt.Id == entry.Id)
                {
                    oldEntry = mt;
                }
            }

            entries.Remove(oldEntry);

            entries.Add(entry);

            List<string> lines = new List<string>();

            foreach (MatchupEntryModel me in entries)
            {
                string parent = "";
                if (me.ParentMatch != null)
                {
                    parent = me.ParentMatch.Id.ToString();
                }
                string teamCompeting = "";
                if (me.TeamCompeting != null)
                {
                    teamCompeting = me.TeamCompeting.Id.ToString();
                }
                lines.Add($"{ me.Id },{ teamCompeting },{ me.Score },{ parent }");
            }

            File.WriteAllLines(GlobalConfig.MatchUpEntryFile.FullFilePath(), lines);
        }
        public static void SaveEntryToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchUpEntryFile.FullFilePath().LoadFile().ConvertToMatchUpEntryModels();
            int currentId = 1;

            if (entries.Count > 0)
            {
                currentId = entries.OrderByDescending(x => x.Id).First().Id + 1;
            }

            entry.Id = currentId;

            entries.Add(entry);

            List<string> lines = new List<string>();

            foreach (MatchupEntryModel me in entries)
            {
                string parent = "";
                if (me.ParentMatch != null)
                {
                    parent = me.ParentMatch.Id.ToString();
                }
                string teamCompeting = "";
                if (me.TeamCompeting != null)
                {
                    teamCompeting = me.TeamCompeting.Id.ToString();
                }
                lines.Add($"{ me.Id },{ teamCompeting },{ me.Score },{ parent }");
            }

            File.WriteAllLines(GlobalConfig.MatchUpEntryFile.FullFilePath(), lines);
        }
        private static TeamModel LookUpTeamById(int Id)
        {
            List<string> teams = GlobalConfig.TeamFile.FullFilePath().LoadFile();

            foreach (string team in teams)
            {
                string[] cols = team.Split(',');
                if (cols[0] == Id.ToString())
                {
                    List<string> matchingTeams = new List<string>();
                    matchingTeams.Add(team);
                    return matchingTeams.ConvertToTeamModel().First();
                }
            }

            return null;
        }
        private static List<MatchupEntryModel> ConvertStringToMatchUpEntryModels(string input)
        {
            string[] ids = input.Split('|');
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            List<string> entries = GlobalConfig.MatchUpEntryFile.FullFilePath().LoadFile();
            List<string> matchingEntries = new List<string>();

            foreach (string Id in ids)
            {
                foreach (string entry in entries)
                {
                    string[] col = entry.Split(',');
                    if (col[0] == Id)
                    {
                        matchingEntries.Add(entry);
                    }
                }
            }
            output = matchingEntries.ConvertToMatchUpEntryModels();
            return output;
        }

        private static MatchupModel LookUpMatchUpById(int Id)
        {
            List<string> matchups = GlobalConfig.MatchUpFile.FullFilePath().LoadFile();

            foreach (string matchup in matchups)
            {
                string[] cols = matchup.Split(',');
                if (cols[0] == Id.ToString())
                {
                    List<string> matchingMatch = new List<string>();
                    matchingMatch.Add(matchup);
                    return matchingMatch.ConvertToMatchaUpModels().First();
                }
            }

            return null;
        }
        public static List<MatchupEntryModel> ConvertToMatchUpEntryModels(this List<string> Lines)
        {
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            foreach (string line in Lines)
            {
                string[] col = line.Split(',');

                MatchupEntryModel ms = new MatchupEntryModel();
                ms.Id = int.Parse(col[0]);
                if (col[1].Length == 0)
                {
                    ms.TeamCompeting = null;
                }
                else
                {
                    ms.TeamCompeting = LookUpTeamById(int.Parse(col[2]));
                }
                ms.Score = double.Parse(col[2]);
                int parentId = 0;
                if (int.TryParse(col[3], out parentId))
                {
                    ms.ParentMatch = LookUpMatchUpById(parentId);
                }
                else
                {
                    ms.ParentMatch = null;
                }
                output.Add(ms);
            }

            return output;
        }
        public static List<MatchupModel> ConvertToMatchaUpModels(this List<string> Lines)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            foreach (string line in Lines)
            {
                string[] col = line.Split(',');

                MatchupModel per = new MatchupModel();
                per.Id = int.Parse(col[0]);
                per.Entries = ConvertStringToMatchUpEntryModels(col[1]);
                if (col[2].Length == 0)
                {
                    per.Winner = null;
                }
                else
                {
                    per.Winner = LookUpTeamById(int.Parse(col[2]));
                }
                per.MatchupRound = int.Parse(col[3]);
                output.Add(per);
            }

            return output;
        }
        public static void SaveToPrizeFile(this List<PrizeModel> models)
        {
            List<string> lines = new List<string>();

            foreach (PrizeModel p in models)
            {
                lines.Add($"{ p.Id},{ p.PlaceNumber },{ p.PlaceName },{ p.PrizeAmount },{ p.PrizePercentage }");
            }

            File.WriteAllLines(GlobalConfig.PrizesFile.FullFilePath(), lines);
        }

        public static void SaveToPeopleFile(this List<PersonModel> models)
        {
            List<string> lines = new List<string>();

            foreach (PersonModel pers in models)
            {
                lines.Add($"{ pers.Id},{ pers.FirstName },{ pers.LastName },{ pers.EmailAddress },{ pers.CellPhoneNumber }");
            }

            File.WriteAllLines(GlobalConfig.PeopleFile.FullFilePath(), lines);
        }

        public static void SaveToTeamFile(this List<TeamModel> models)
        {
            List<string> lines = new List<string>();

            foreach (TeamModel pers in models)
            {
                lines.Add($"{ pers.Id},{ pers.TeamName },{ ConvertPeopleListToString(pers.TeamMembers) }");
            }

            File.WriteAllLines(GlobalConfig.TeamFile.FullFilePath(), lines);
        }

        public static void SaveToTournamentFile(this List<TournamentModel> models)
        {
            List<string> lines = new List<string>();

            foreach (TournamentModel pers in models)
            {
                lines.Add($"{ pers.Id},{ pers.TournamentName },{ pers.EntryFee },{ ConvertTeamListToString(pers.EnteredTeams) },{ ConvertPrizesListToString(pers.Prizes) },{ ConvertRoundsListToString(pers.Rounds)}");
            }

            File.WriteAllLines(GlobalConfig.TournamentFile.FullFilePath(), lines);
        }
        private static string ConvertMatchupEntriesToString(List<MatchupEntryModel> entries)
        {
            string output = "";
            if (entries.Count == 0)
            {
                return "";
            }
            foreach (MatchupEntryModel en in entries)
            {
                output += $"{en.Id}|";
            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }
        private static string ConvertRoundsListToString(List<List<MatchupModel>> rounds)
        {
            string output = "";
            if (rounds.Count == 0)
            {
                return "";
            }
            foreach (List<MatchupModel> rs in rounds)
            {
                output += $"{ConvertMatchUpToStrings(rs) }^|";
            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertMatchUpToStrings(List<MatchupModel> matchups)
        {
            string output = "";
            if (matchups.Count == 0)
            {
                return "";
            }
            foreach (MatchupModel m in matchups)
            {
                output += $"{ m.Id }^";
            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }
        private static string ConvertTeamListToString(List<TeamModel> teams)
        {
            string output = "";
            if (teams.Count == 0)
            {
                return "";
            }
            foreach (TeamModel tm in teams)
            {
                output += $"{tm.Id}|";
            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertPrizesListToString(List<PrizeModel> prize)
        {
            string output = "";
            if (prize.Count == 0)
            {
                return "";
            }
            foreach (PrizeModel p in prize)
            {
                output += $"{p.Id}|";
            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }
        private static string ConvertPeopleListToString(List<PersonModel> people)
        {
            string output = "";
            if (people.Count == 0)
            {
                return "";
            }
            foreach (PersonModel p in people)
            {
                output += $"{p.Id}|";
            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }
    }
}