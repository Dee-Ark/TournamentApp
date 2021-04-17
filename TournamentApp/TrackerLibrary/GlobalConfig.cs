﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAcces;

namespace TrackerLibrary
{
    public static class GlobalConfig
    {
        public const string PrizesFile = "PrizeModels.csv";
        public const string PeopleFile = "PersonModels.csv";
        public const string TeamFile = "TeamModels.csv";
        public const string TournamentFile = "TournamentModels.csv";
        public const string MatchUpFile = "MatchupModel.csv";
        public const string MatchUpEntryFile = "MatchupEntryModels.csv";

        //public static List<IDataConnection> Connection { get; private set; } = new List<IDataConnection>();
        public static IDataConnection Connection { get; private set; }
        //public static object TeamFile { get; internal set; }

        public static void InitaizeConnections(DatabaseType db)
        {
            if (db == DatabaseType.Sql)
            {
                //TODO - Set up the SQL Connector properly
                SqlConnector sql = new SqlConnector();
                Connection = sql;
            }

            else if (db == DatabaseType.TextFile)
            {
                //TODO - Create the Text Connection
                TextConnector text = new TextConnector();
                Connection = text;
            }
        }

        public static string CnnString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public static string AppKeyLookUp(string Key)
        {
            return ConfigurationManager.AppSettings[Key];
        }
    }
}