using System;
using System.ComponentModel;

namespace Civ4_Leader_Changer
{
    public enum LeaderType
    {
        NONE,
        LEADER_ALEXANDER,
        LEADER_ASOKA,
        LEADER_AUGUSTUS,
        LEADER_BISMARCK,
        LEADER_BOUDICA,
        LEADER_BRENNUS,
        LEADER_CATHERINE,
        LEADER_CHARLEMAGNE,
        LEADER_CHURCHILL,
        LEADER_CYRUS,
        LEADER_DARIUS,
        LEADER_DE_GAULLE,
        LEADER_ELIZABETH,
        LEADER_FREDERICK,
        LEADER_GANDHI,
        LEADER_GENGHIS_KHAN,
        LEADER_GILGAMESH,
        LEADER_HAMMURABI,
        LEADER_HANNIBAL,
        LEADER_HATSHEPSUT,
        LEADER_HUAYNA_CAPAC,
        LEADER_ISABELLA,
        LEADER_JOAO,
        LEADER_JULIUS_CAESAR,
        LEADER_JUSTINIAN,
        LEADER_KUBLAI_KHAN,
        LEADER_LINCOLN,
        LEADER_LOUIS_XIV,
        LEADER_MANSA_MUSA,
        LEADER_CHINESE_LEADER,
        LEADER_MEHMED,
        LEADER_MONTEZUMA,
        LEADER_NAPOLEON,
        LEADER_PACAL,
        LEADER_PERICLES,
        LEADER_PETER,
        LEADER_QIN_SHI_HUANG,
        LEADER_RAMESSES,
        LEADER_RAGNAR,
        LEADER_FRANKLIN_ROOSEVELT,
        LEADER_SALADIN,
        LEADER_SHAKA,
        LEADER_SITTING_BULL,
        LEADER_STALIN,
        LEADER_SULEIMAN,
        LEADER_SURYAVARMAN,
        LEADER_TOKUGAWA,
        LEADER_VICTORIA,
        LEADER_WANGKON,
        LEADER_WASHINGTON,
        LEADER_WILLEM_VAN_ORANJE,
        LEADER_ZARA_YAQOB
    }

    [Flags]
    public enum Tech
    {
        NONE = 0b0000000,
        THE_WHEEL = 0b0000001,
        AGRICULTURE = 0b0000010,
        HUNTING = 0b0000100,
        ARCHERY = 0b0001000,
        FISHING = 0b0010000,
        MINING = 0b0100000,
        MYSTICISM = 0b1000000
    }

    public class WorldbuilderLeader : INotifyPropertyChanged
    {
        private LeaderType type;
        public LeaderType Type
        {
            get { return this.type; }
            set
            {
                if (this.type != value)
                {
                    this.type = value;
                    this.CopyFrom(WorldbuilderLeaderData.leaderDictionary[value]);
                    this.NotifyPropertyChanged("Type");
                }
            }
        }
        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    if (value != "")
                        this.name = value;
                    else
                        this.name = this.defaultName;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }
        private PlayerColor color;
        public PlayerColor Color
        {
            get { return this.color; }
            set
            {
                if (this.color != value)
                {
                    this.color = value;
                    this.NotifyPropertyChanged("Color");
                }
            }
        }
        public int teamNumber;
        public int startingX;
        public int startingY;
        private Tech techs;
        public Tech Techs
        {
            get { return this.techs; }
            set
            {
                if (this.techs != value)
                {
                    this.techs = value;
                    this.NotifyPropertyChanged("Techs");
                }
            }
        }
        public string handicap;

        public string leaderType { get; private set; }
        public string defaultName { get; private set; }
        public string civDesc { get; private set; }
        public string civShortDesc { get; private set; }
        public string civAdjective { get; private set; }
        public string flagDecal { get; private set; }
        public string civType { get; private set; }
        public PlayerColor defaultColor { get; private set; }
        public string artStyle { get; private set; }
        public Tech defaultTechs { get; private set; }

        public WorldbuilderLeader()
        {
            this.Type = LeaderType.NONE;
        }

        // This constructor is only used to populate the leaderDictionary with default stats for the leaders
        public WorldbuilderLeader(LeaderType type, string leaderType, string defaultName, string civDesc, string civShortDesc,
            string civAdjective, string flagDecal, string civType, PlayerColor defaultColor, string artStyle, Tech defaultTechs)
        {
            this.type = type;
            this.leaderType = leaderType;
            this.defaultName = defaultName;
            this.civDesc = civDesc;
            this.civShortDesc = civShortDesc;
            this.civAdjective = civAdjective;
            this.flagDecal = flagDecal;
            this.civType = civType;
            this.defaultColor = defaultColor;
            this.artStyle = artStyle;
            this.defaultTechs = defaultTechs;
        }

        public void CopyFrom(WorldbuilderLeader other)
        {
            this.Name = other.defaultName;
            this.Color = other.defaultColor;
            this.Techs = other.defaultTechs;

            this.Type = other.type;
            this.leaderType = other.leaderType;
            this.defaultName = other.defaultName;
            this.civDesc = other.civDesc;
            this.civShortDesc = other.civShortDesc;
            this.civAdjective = other.civAdjective;
            this.flagDecal = other.flagDecal;
            this.civType = other.civType;
            this.defaultColor = other.defaultColor;
            this.artStyle = other.artStyle;
            this.defaultTechs = other.defaultTechs;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
