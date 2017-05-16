using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCONtest
{
    enum PlayerStatus
    {
        invalid=0,
        owner,
        leader,
        member,
        friend,
        pub
    };

    class Player
    {
        private string name;
        private string guid;
        private int kills;
        private int deaths;
        private int slotNumber;
        private PlayerStatus status;
        private int teamKills;
        private int warnCount;

        public string Name { get { return this.name; } set { this.name = value; } }
        public string GUID { get { return this.guid; } set { this.guid = value; } }
        public int Kills { get { return this.kills; } set { this.kills = value; } }
        public int Deaths { get { return this.deaths; } set { this.deaths = value; } }
        public int SlotNumber { get { return this.slotNumber; } set { this.slotNumber = value; } }
        public PlayerStatus Status { get { return this.status; } set { this.status = value; } }
        public int TeamKills { get { return this.teamKills; } set { this.teamKills = value; } }
        public int WarnCount { get { return this.warnCount; } set { this.warnCount = value; }
        }
                        
        public Player(string N, string ID, int slotNum)
        {
            this.name = N;
            this.guid = ID;
            this.kills = 0;
            this.deaths = 0;
            this.slotNumber = slotNum;
            this.status = PlayerStatus.invalid;
            this.teamKills = 0;
            this.warnCount = 0;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public void Clear()
        {
            kills = 0;
            deaths = 0;
            teamKills = 0;
        }
    }
}
