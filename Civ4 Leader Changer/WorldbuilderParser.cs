using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Civ4_Leader_Changer
{
    // Parser (and writer) for the Worldbuilder save files.
    // Main attractions are the "ParseWorldbuilderSave" and "WriteWorldbuilderSave" methods.
    // This class also holds the "leaders" container which holds the list of leaders read from the save file
    class WorldbuilderParser
    {
        // Helper function: Adds all the Deity difficulty level bonus starting techs (also defined here) to the leaders
        private const Tech deityTechs = Tech.THE_WHEEL | Tech.AGRICULTURE | Tech.HUNTING | Tech.ARCHERY;
        public void AddDeityTechs()
        {
            foreach (var l in leaders)
            {
                if (l.Type != LeaderType.NONE)
                    l.Techs |= deityTechs;
            }
        }
        // Helper function: Restore all the leaders to only their default starting techs
        public void SetDefaultTechs()
        {
            foreach (var l in leaders)
                l.Techs = l.defaultTechs;
        }

        // An attempt to reduce the number of "magic values" in the methods below. Not sure if successful, or if 
        //  it would really improve readability if I went all in... :/
        const string VERSION = "11";
        const string BEGIN_PREAMBLE = "BeginGame";
        const string END_PREAMBLE = "EndGame";

        const int NUM_TEAMS = 18;
        const string BEGIN_TEAM = "BeginTeam";
        const string END_TEAM = "EndTeam";

        const string BEGIN_LEADER = "BeginPlayer";
        const string END_LEADER = "EndPlayer";

        // The three parts of the savegame that I read and store. Roughly the savegame is structured as:
        //      Game options
        //      Team definitions with their starting techs - these I do NOT store
        //      Leaders
        //      Mapdata
        private string preamble;
        internal List<WorldbuilderLeader> leaders;
        private string mapData;
        // Guard to prevent a save on bad data
        private bool loadedData;

        // Default constructor for the parser
        internal WorldbuilderParser()
        {
            loadedData = false;
            leaders = new List<WorldbuilderLeader>();
        }

        // The first of the two major methods in the class. This one loads a Worldbuilder save in through a filename.
        internal bool ParseWorldbuilderSave(string filename)
        {
            // We are about to load a game - don't trust the data we have until we are done
            loadedData = false;

            // Open a StreamReader on the savegame filename
            using (StreamReader sr = new StreamReader(filename))
            {
                // Make a StringBuilder for collecting the preamble
                StringBuilder sb = new StringBuilder();

                // Sanity check on start line
                string line = sr.ReadLine();
                if (line != $"Version={VERSION}")
                    return false;
                sb.AppendLine(line);

                // Read the preamble into the StringBuilder - basically everything between "BeginGame" and "EndGame"
                if (!ReadPreamble(sr, sb))
                    return false;
                // Store the preamble
                preamble = sb.ToString();

                // Read the teams and discard - this part will not need to be stored
                for (int i = 0; i < NUM_TEAMS; i++)
                {
                    if (!DiscardTeam(sr))
                        return false;
                }

                // Read the leaders one at a time, and store them in the "leaders" list
                WorldbuilderLeader leader;
                for (int i = 0; i < NUM_TEAMS; i++)
                {
                    if (ReadLeader(sr, out leader))
                        leaders.Add(leader);
                    else
                        return false;
                }

                // Finally read the map data - easy because we don't care what it says, we just want to store it
                mapData = sr.ReadToEnd();
            }

            // We have loaded a game
            loadedData = true;
            // Return notice of success. Any errors encountered earlier would have returned false
            return true;
        }

        // Private helper for reading the preamble
        private bool ReadPreamble(StreamReader sr, StringBuilder sb)
        {
            // Check if we have a "BeginGame" line
            string line = sr.ReadLine();
            if (line != BEGIN_PREAMBLE)
                return false;
            sb.AppendLine(line);

            // Read the rest line by line until we reach the end marker, then return success
            while ((line = sr.ReadLine()) != null)
            {
                sb.AppendLine(line);
                if (line == END_PREAMBLE)
                    return true;
            }

            // We made it to the end without encountering "EndGame": Return error.
            return false;
        }

        // Private helper for reading (and discarding) a single team and starting tech section
        private bool DiscardTeam(StreamReader sr)
        {
            // Check if we have a "BeginTeam" line
            string line = sr.ReadLine();
            if (line != BEGIN_TEAM)
                return false;

            // Read the rest line by line until we reach the end marker, then return success
            while ((line = sr.ReadLine()) != null)
            {
                if (line == END_TEAM)
                    return true;
            }

            // We made it to the end without encountering "EndTeam": Return error.
            return false;
        }

        // Private helper for reading a single leader
        private bool ReadLeader(StreamReader sr, out WorldbuilderLeader leader)
        {
            // The leader we are going to build as we read
            leader = new WorldbuilderLeader();

            // Check if we have a "BeginPlayer" line
            string line = sr.ReadLine();
            if (line != BEGIN_LEADER)
                return false;

            // Read the leader data line by line
            while ((line = sr.ReadLine()) != null)
            {
                // Switch on the line contents - most of the lines we don't need to store, so just look for the ones we do
                switch (line)
                {
                    case var s when s.StartsWith("	Team="):
                        // Save the team number - they should be in numeric order in the file, but it is easy to save and maybe
                        //  they appear out of order sometimes?
                        leader.teamNumber = Int32.Parse(s.Substring(s.IndexOf('=') + 1));
                        break;
                    case var s when s.StartsWith("	LeaderName="):
                        // Save the leader name - just to be nice, so you don't have to set it again later
                        leader.Name = s.Substring(s.IndexOf('=') + 1);
                        break;
                    case var s when s.StartsWith("	LeaderType="):
                        // The most important part of the block - this one determins (almost) everything else!
                        LeaderType t;
                        // Sanity check on the string to make sure it is valid
                        if (Enum.TryParse(s.Substring(s.IndexOf('=') + 1), out t))
                            leader.CopyFrom(WorldbuilderLeaderData.leaderDictionary[t]);
                        else
                            return false;
                        break;
                    case var s when s.StartsWith("	StartingX="):
                        // Save the starting location. No idea if this does anything once the units are on the map, but it is easy to save
                        string tmp = s.Substring(s.IndexOf('=') + 1);
                        leader.startingX = Int32.Parse(tmp.Substring(0, tmp.IndexOf(',')));
                        leader.startingY = Int32.Parse(tmp.Substring(tmp.IndexOf('=') + 1));
                        break;
                    case var s when s.StartsWith("	Handicap="):
                        // Another setting that don't really matter (can be set when you start the scenario), but it is an easy quality of life thing to save 
                        leader.handicap = s.Substring(s.IndexOf('=') + 1);
                        break;
                    case END_LEADER:
                        // We read the entire leader block - return success!
                        return true;
                    default:
                        // Unimportant line - move on
                        break;
                }
            }

            // We made it to the end without encountering "EndPlayer": Return error.
            return false;
        }

        // The second major mathod in the class. This one saves the loaded data to a file again.
        internal void WriteWorldbuilderSave(string filename)
        {
            // If we don't have a game loaded, then don't try to save
            if (!loadedData)
                return;

            // Open a StreamWriter on the given filename
            using (StreamWriter sw = new StreamWriter(filename))
            {
                // Write the stored preamble
                sw.Write(preamble);

                // Write the team + starting techs sections. One for each leader (including the type = NONE leaders)
                foreach (var leader in leaders)
                {
                    WriteTeam(sw, leader);
                }

                // Write the leader sections. One for each leader (including the type = NONE leaders)
                foreach (var leader in leaders)
                {
                    WriteLeader(sw, leader);
                }

                // Write the stored mapdata
                sw.Write(mapData);
            }
        }

        // Helper method: Writes a single Team section
        private void WriteTeam(StreamWriter sw, WorldbuilderLeader leader)
        {
            /*
             * A team section looks something like this:
             * 
                BeginTeam
	                TeamID=0
	                Tech=TECH_AGRICULTURE
	                Tech=TECH_FISHING
	                ContactWithTeam=0
	                RevealMap=0
                EndTeam
             */

            sw.WriteLine(BEGIN_TEAM);
            sw.WriteLine($"\tTeamID={leader.teamNumber}");
            foreach (Tech t in Enum.GetValues(typeof(Tech)))
            {
                // Break the "leader.Tech" field down into individual bits (or techs)
                if ((leader.Techs & t) != Tech.NONE)
                    // The members of the "Tech" enum are named after the strings that appear in the savefile, except
                    //  that the preceeding "TECH_" has been removed - it is added back here
                    sw.WriteLine($"\tTech=TECH_{Enum.GetName(typeof(Tech), t)}");
            }
            sw.WriteLine($"\tContactWithTeam={leader.teamNumber}");
            sw.WriteLine("\tRevealMap=0");
            sw.WriteLine(END_TEAM);
        }

        // Helper method: Writes a single leader section
        private void WriteLeader(StreamWriter sw, WorldbuilderLeader leader)
        {
            if (leader.Type == LeaderType.NONE)
            {
                // The "NONE" type leaders have a very reduced block - the only part that we have stored is the team number
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
                // If a name is set use that, else use the default name
                sw.WriteLine($"\tLeaderName={(leader.Name != "" ? leader.Name : leader.defaultName)}");
                sw.WriteLine($"\tCivDesc={leader.civDesc}");
                sw.WriteLine($"\tCivShortDesc={leader.civShortDesc}");
                sw.WriteLine($"\tCivAdjective={leader.civAdjective}");
                sw.WriteLine($"\tFlagDecal={leader.flagDecal}");
                sw.WriteLine($"\tWhiteFlag=0");
                sw.WriteLine($"\tCivType={leader.civType}");
                // The PlayerColor enum entries are named after the strings in the save file, but for readability the
                //  "PLAYERCOLOR_" prefix was removed - here it is added again to make the strings correct in the save
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
