using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Civ4_Leader_Changer
{
    class WorldbuilderParser
    {
        private const Tech deityTechs = Tech.THE_WHEEL | Tech.AGRICULTURE | Tech.HUNTING | Tech.ARCHERY;
        public void AddDeityTechs()
        {
            foreach (var l in leaders)
            {
                if (l.Type != LeaderType.NONE)
                    l.Techs |= deityTechs;
            }
        }
        public void SetDefaultTechs()
        {
            foreach (var l in leaders)
                l.Techs = l.defaultTechs;
        }

        const string VERSION = "11";
        const string BEGIN_PREAMBLE = "BeginGame";
        const string END_PREAMBLE = "EndGame";

        const int NUM_TEAMS = 18;
        const string BEGIN_TEAM = "BeginTeam";
        const string END_TEAM = "EndTeam";

        const string BEGIN_LEADER = "BeginPlayer";
        const string END_LEADER = "EndPlayer";

        private string preamble;
        internal List<WorldbuilderLeader> leaders;
        private string mapData;

        internal WorldbuilderParser()
        {
            leaders = new List<WorldbuilderLeader>();
        }

        internal bool ParseWorldbuilderSave(string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                StringBuilder sb = new StringBuilder();

                // Sanity check on start line
                string line = sr.ReadLine();
                if (line != $"Version={VERSION}")
                    return false;
                sb.AppendLine(line);

                // Read the preamble
                if (!ReadPreamble(sr, sb))
                    return false;
                preamble = sb.ToString();
                sb.Clear();

                // Read the teams and discard - this part will not need to be stored
                for (int i = 0; i < NUM_TEAMS; i++)
                {
                    if (!DiscardTeam(sr))
                        return false;
                }

                // Read the leaders
                WorldbuilderLeader leader;
                for (int i = 0; i < NUM_TEAMS; i++)
                {
                    if (ReadLeader(sr, out leader))
                        leaders.Add(leader);
                    else
                        return false;
                }

                mapData = sr.ReadToEnd();
            }
            return true;
        }

        private bool ReadPreamble(StreamReader sr, StringBuilder sb)
        {
            string line = sr.ReadLine();

            // Check if we have a "BeginGame" line
            if (line != BEGIN_PREAMBLE)
                return false;
            sb.AppendLine(line);

            while ((line = sr.ReadLine()) != null)
            {
                sb.AppendLine(line);
                if (line == END_PREAMBLE)
                    return true;
            }

            // We made it to the end without encountering "EndGame": Return error.
            return false;
        }

        private bool DiscardTeam(StreamReader sr)
        {
            string line = sr.ReadLine();

            // Check if we have a "BeginTeam" line
            if (line != BEGIN_TEAM)
                return false;

            while ((line = sr.ReadLine()) != null)
            {
                if (line == END_TEAM)
                    return true;
            }

            // We made it to the end without encountering "EndTeam": Return error.
            return false;
        }

        private bool ReadLeader(StreamReader sr, out WorldbuilderLeader leader)
        {
            leader = new WorldbuilderLeader();

            string line = sr.ReadLine();

            // Check if we have a "BeginPlayer" line
            if (line != BEGIN_LEADER)
                return false;

            while ((line = sr.ReadLine()) != null)
            {
                switch (line)
                {
                    case var s when s.StartsWith("	Team="):
                        leader.teamNumber = Int32.Parse(s.Substring(s.IndexOf('=') + 1));
                        break;
                    case var s when s.StartsWith("	LeaderName="):
                        leader.Name = s.Substring(s.IndexOf('=') + 1);
                        break;
                    case var s when s.StartsWith("	LeaderType="):
                        LeaderType t;
                        if (Enum.TryParse(s.Substring(s.IndexOf('=') + 1), out t))
                            leader.CopyFrom(WorldbuilderLeaderData.leaderDictionary[t]);
                        else
                            return false;
                        break;
                    case var s when s.StartsWith("	StartingX="):
                        string tmp = s.Substring(s.IndexOf('=') + 1);
                        leader.startingX = Int32.Parse(tmp.Substring(0, tmp.IndexOf(',')));
                        leader.startingY = Int32.Parse(tmp.Substring(tmp.IndexOf('=') + 1));
                        break;
                    case var s when s.StartsWith("	Handicap="):
                        leader.handicap = s.Substring(s.IndexOf('=') + 1);
                        break;
                    case END_LEADER:
                        return true;
                    default:
                        break;
                }
            }

            // We made it to the end without encountering "EndPlayer": Return error.
            return false;
        }

        internal void WriteWorldbuilderSave(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(preamble);

                foreach (var leader in leaders)
                {
                    WriteTeam(sw, leader);
                }

                foreach (var leader in leaders)
                {
                    WriteLeader(sw, leader);
                }

                sw.Write(mapData);
            }
        }

        private void WriteTeam(StreamWriter sw, WorldbuilderLeader leader)
        {
            sw.WriteLine(BEGIN_TEAM);
            sw.WriteLine($"\tTeamID={leader.teamNumber}");
            foreach (Tech t in Enum.GetValues(typeof(Tech)))
            {
                if ((leader.Techs & t) != Tech.NONE)
                    sw.WriteLine($"\tTech=TECH_{Enum.GetName(typeof(Tech), t)}");
            }
            sw.WriteLine($"\tContactWithTeam={leader.teamNumber}");
            sw.WriteLine("\tRevealMap=0");
            sw.WriteLine(END_TEAM);
        }

        private void WriteLeader(StreamWriter sw, WorldbuilderLeader leader)
        {
            if (leader.Type == LeaderType.NONE)
            {
                sw.WriteLine($"{BEGIN_LEADER}");
                sw.WriteLine($"\tTeam={leader.teamNumber}");
                sw.WriteLine($"\tLeaderType=NONE");
                sw.WriteLine($"\tCivType=NONE");
                sw.WriteLine($"\tColor=NONE");
                sw.WriteLine($"\tArtStyle=NONE");
                sw.WriteLine($"\tHandicap=HANDICAP_NOBLE");
                sw.WriteLine($"{END_LEADER}");
            }
            else
            {
                sw.WriteLine($"{BEGIN_LEADER}");
                sw.WriteLine($"\tTeam={leader.teamNumber}");
                sw.WriteLine($"\tLeaderType={leader.leaderType}");
                sw.WriteLine($"\tLeaderName={(leader.Name != "" ? leader.Name : leader.defaultName)}");
                sw.WriteLine($"\tCivDesc={leader.civDesc}");
                sw.WriteLine($"\tCivShortDesc={leader.civShortDesc}");
                sw.WriteLine($"\tCivAdjective={leader.civAdjective}");
                sw.WriteLine($"\tFlagDecal={leader.flagDecal}");
                sw.WriteLine($"\tWhiteFlag=0");
                sw.WriteLine($"\tCivType={leader.civType}");
                sw.WriteLine($"\tColor=PLAYERCOLOR_{Enum.GetName(typeof(PlayerColor), leader.Color)}");
                sw.WriteLine($"\tArtStyle={leader.artStyle}");
                sw.WriteLine($"\tPlayableCiv=1");
                sw.WriteLine($"\tMinorNationStatus=0");
                sw.WriteLine($"\tStartingGold=0");
                sw.WriteLine($"\tStartingX={leader.startingX}, StartingY={leader.startingY}");
                sw.WriteLine($"\tStateReligion=");
                sw.WriteLine($"\tStartingEra=ERA_ANCIENT");
                sw.WriteLine($"\tRandomStartLocation=false");
                sw.WriteLine($"\tCivicOption=CIVICOPTION_GOVERNMENT, Civic=CIVIC_DESPOTISM");
                sw.WriteLine($"\tCivicOption=CIVICOPTION_LEGAL, Civic=CIVIC_BARBARISM");
                sw.WriteLine($"\tCivicOption=CIVICOPTION_LABOR, Civic=CIVIC_TRIBALISM");
                sw.WriteLine($"\tCivicOption=CIVICOPTION_ECONOMY, Civic=CIVIC_DECENTRALIZATION");
                sw.WriteLine($"\tCivicOption=CIVICOPTION_RELIGION, Civic=CIVIC_PAGANISM");
                sw.WriteLine($"\tHandicap={leader.handicap}");
                sw.WriteLine($"{END_LEADER}");
            }
        }
    }
}
