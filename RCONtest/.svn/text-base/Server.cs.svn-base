using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql;
using MySql.Data;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace RCONtest
{
    using System.Data;
    using System.Reflection;

    using MySql.Data.MySqlClient;

    class Server
    {
        private string IP;

        private int port;

        private string rconPW;

        private string MySQLIP;

        private string databaseName;

        private string databaseUser;

        private string databasePW;

        private Socket gameServer;

        private MySqlConnection SQLconnection;

        private Dictionary<string, Player> currentPlayers;

        private LogParsing lp;

        private List<Maps> mapNames;

        private List<GameTypes> gameTypes;

        private List<RotationEntry> gameRotation;

        private readonly string[] commandList = { "test", "warn", "about", "nextmap", "setmap", "setgamemode", "vote", "startvote", "stats", "playerlist", "rules",
                                       "kick", "clientkick", "tempbanuser", "tempbanclient", "banuser", "banclient", "say", "playlist", "setadminvar", "tell"};

        public Server(string ip, int p, string rconpw, string sqlIP, string dbName, string dbUser, string dbPW, string serverLog)
        {
            this.IP = ip;
            this.port = p;
            this.databaseName = dbName;
            this.databaseUser = dbUser;
            this.databasePW = dbPW;
            this.MySQLIP = sqlIP;
            this.currentPlayers = new Dictionary<string, Player>();
            this.rconPW = rconpw;

            this.SQLConnect();
            this.lp = new LogParsing(serverLog, this);
            this.lp.Parse();

            this.mapNames = new List<Maps>();
            this.gameTypes = new List<GameTypes>();
            this.gameRotation = new List<RotationEntry>();
        }

        public bool RCONConnect()
        {
            try
            {
                this.gameServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                this.gameServer.Connect(this.IP, this.port);

                return this.gameServer.Connected;
            }
            catch (Exception ex)
            {
                Console.Write("errored on RCON connect");
                return false;
            }
        }

        public bool PlayerJoin(Player p)
        {
            if (this.currentPlayers.ContainsKey(p.GUID) || p.Name.ToLower() == "[3arc]democlient")
            {
                return false;
            }

            this.currentPlayers.Add(p.GUID, p);
            return true;
            
        }

        public void PlayerJoin(string pname, string pGUID, string slotNum)
        {
            this.PlayerJoin(new Player(pname, pGUID, Convert.ToInt32(slotNum)));
        }

        public void PlayerQuit(Player p)
        {
            this.PlayerQuit(p.GUID);
        }

        public void PlayerQuit(string pGUID)
        {
            if (this.currentPlayers.ContainsKey(pGUID))
            {
                this.currentPlayers.Remove(pGUID);
            }
        }

        private void SQLConnect()
        {
            if (this.SQLconnection != null && this.SQLconnection.State == ConnectionState.Open)
            {
                return;
            }

            var myConString = "SERVER=" + this.MySQLIP + ";" + "DATABASE=" + this.databaseName + ";" + "UID=" +
                              this.databaseUser + ";" + "PASSWORD=" + this.databasePW + "; check parameters=false;";

            this.SQLconnection = new MySqlConnection(myConString);

            // var command = this.SQLconnection.CreateCommand();
            // command.CommandText = "select * from weapons";
            // this.SQLconnection.Open();
            // var reader = command.ExecuteReader();
            // while (reader.Read())
            // {
            //    var thisrow = string.Empty;
            //    for (var i = 0; i < reader.FieldCount; i++)
            //    {
            //        thisrow += reader.GetValue(i).ToString() + ",";
            //    }
            //    Console.WriteLine(thisrow);
            // }

            // SQLconnection.Close();
        }

        public PlayerStatus PlayerStatusCheck(string GUID)
        {
            return PlayerStatus.invalid;
        }

        public int AlterPlayerTeamKillCount(string GUID, int i)
        {
            if (this.PlayerPresent(GUID))
            {
                this.currentPlayers[GUID].TeamKills += i;
                return this.currentPlayers[GUID].TeamKills;
            }
            return -1;
        }

        public int AlterPlayerKillCount(string GUID, int i)
        {
            if (this.PlayerPresent(GUID))
            {
                this.currentPlayers[GUID].Kills += i;
                return this.currentPlayers[GUID].Kills;
            }
            return -1;
        }

        public int AlterPlayerDeathCount(string GUID, int i)
        {
            if (this.PlayerPresent(GUID))
            {
                this.currentPlayers[GUID].Deaths += i;
                return this.currentPlayers[GUID].Deaths;
            }
            return -1;
        }

        public int AlterPlayerWarnCount(string GUID, int i)
        {
            if (this.PlayerPresent(GUID))
            {
                this.currentPlayers[GUID].WarnCount += i;
                return this.currentPlayers[GUID].WarnCount;
            }
            return -1;
        }

        public bool PlayerPresent(string GUID)
        {
            return this.currentPlayers.ContainsKey(GUID);
        }

        public string PartialPlayerPresent(string partialName)
        {
            var foundName = string.Empty;

            foreach (var currentPlayer in this.currentPlayers)
            {
                if (currentPlayer.Value.Name.Contains(partialName))
                {
                    if (foundName == string.Empty)
                    {
                        foundName = currentPlayer.Value.GUID;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            return foundName;
        }

        public void ParsePlayerCommand(string GUID, string suspectedCommand)
        {
            var exclaimMark = suspectedCommand.IndexOf('!');
            var cmd = "test";

            if (exclaimMark > 1)
            {
                return;
            }

            for (var i = 0; i <= exclaimMark; i++)
            {
                suspectedCommand = suspectedCommand.Remove(0, 1);
                cmd = suspectedCommand.Split(' ')[0];
            }

            if (!this.KnownCommand(cmd))
            {
                cmd = "test";
            }

            var thisType = this.GetType();
            var method = cmd.ToLower() + "CMD";
            var theMethod = thisType.GetMethod(method);
            theMethod.Invoke(this, new object[] { (object)suspectedCommand, (object)GUID, this.rconPW });
        }

        public string sayCMD(string msg, string GUID, string RCONpw)
        {
            if (this.HasPermission(GUID, "say"))
            {
                return this.ExecuteCommand(msg, RCONpw);
            }


            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " You do not have permission to say", GUID, RCONpw);
        }

        public string warnCMD(string msg, string GUID, string RCONpw)
        {
            if (this.HasPermission(GUID, "warn"))
            {
                var name = msg.Split(' ')[1];

                if (name.Length < 3)
                {
                    return this.tellCMD("tell " + this.currentPlayers[GUID].Name + " player name too short to make comparison, enter at least 3 characters", GUID, RCONpw);
                }

                var foundName = this.PartialPlayerPresent(name);

                if (foundName == string.Empty)
                {
                    return this.tellCMD("tell " + this.currentPlayers[GUID].Name + " no player name or multiple names found", GUID, RCONpw);
                }


                if (this.currentPlayers[GUID].Status == PlayerStatus.owner || this.currentPlayers[GUID].Status == PlayerStatus.leader)
                {
                    return this.tellCMD("tell " + this.currentPlayers[GUID].Name + " that player is immune to warn", GUID, RCONpw);
                }

                if (this.currentPlayers[foundName].WarnCount >= 4)
                {
                    this.tempbanclientCMD("tempbanuser " + foundName + " warned too much", GUID, RCONpw);
                }

                var splitMsg = msg.Split(' ');
                msg = "tell " + this.currentPlayers[foundName].SlotNumber + " ";
                for (var i = 0; i < splitMsg.Length; i++)
                {
                    if (i > 1)
                    {
                        msg += splitMsg[i] + " ";
                    }
                }

                this.currentPlayers[foundName].WarnCount++;
                return this.ExecuteCommand(msg, RCONpw);
                
            }

            return this.tellCMD("tell " + this.currentPlayers[GUID].Name + " You do not have permission to warn", GUID, RCONpw);
        }

        public string kickCMD(string msg, string GUID, string RCONpw)
        {
            if(this.HasPermission(GUID, "kick"))
            {
                var name = msg.Split(' ')[1];

                if (name.Length < 3)
                {
                    return this.tellCMD("tell " + this.currentPlayers[GUID].Name + " player name too short to make comparison, enter at least 3 characters", GUID, RCONpw);
                }

                var foundName = this.PartialPlayerPresent(name);

                if (foundName == string.Empty)
                {
                    return this.tellCMD("tell " + this.currentPlayers[GUID].Name + " no player name or multiple names found", GUID, RCONpw);
                }

                if (this.currentPlayers[GUID].Status == PlayerStatus.owner || this.currentPlayers[GUID].Status == PlayerStatus.leader)
                {
                    return this.tellCMD("tell " + this.currentPlayers[GUID].Name + " that player is immune to kick", GUID, RCONpw);
                }

                var splitMsg = msg.Split(' ');
                msg = "kick " + this.currentPlayers[foundName].SlotNumber + " ";
                for (var i = 0; i < splitMsg.Length; i++)
                {
                    if (i > 1)
                    {
                        msg += splitMsg[i] + " ";
                    }
                }

                return this.ExecuteCommand(msg, RCONpw);
                
            }

            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " You do not have permission to kick", GUID, RCONpw);
        }

        public string clientkickCMD(string msg, string GUID, string RCONpw)
        {
            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " You do not have permission to clientkick", GUID, RCONpw);
        }

        public string tempbanuserCMD(string msg, string GUID, string RCONpw)
        {
            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " You do not have permission to tempbanuser", GUID, RCONpw);
        }

        public string tempbanclientCMD(string msg, string GUID, string RCONpw)
        {
            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " You do not have permission to tempbanclient", GUID, RCONpw);
        }

        public string banuserCMD(string msg, string GUID, string RCONpw)
        {
            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " You do not have permission to banuser", GUID, RCONpw);
        }

        public string banclientCMD(string msg, string GUID, string RCONpw)
        {
            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " You do not have permission to banclient", GUID, RCONpw);
        }
        
        public string testCMD(string msg, string GUID, string RCONpw)
        {
            if (this.HasPermission(GUID, "test"))
            {
                return
                    this.tellCMD(
                        "tell " + this.currentPlayers[GUID].Name + " This is a test function of the CCOverlord, created by ccrngd1.  If you tried a different command, the command is unknown as you have been directed here",
                        GUID,
                        RCONpw);
            }

            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " You do not have permission", GUID, RCONpw);
        }

        public string aboutCMD(string msg, string GUID, string RCONpw)
        {
            foreach (var cmd in commandList)
            {
                if (this.HasPermission(GUID, cmd))
                {
                    System.Threading.Thread.Sleep(500);
                }

                this.tellCMD("tell " + this.currentPlayers[GUID].Name + " " + cmd, GUID, RCONpw);
            }

            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " CCOverlord brought to you by ccrngd1 - fg.gameservers.com", GUID, RCONpw);
        }

        public string nextmapCMD(string msg, string GUID, string RCONpw)
        {
            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " You do not have permission to nextmap", GUID, RCONpw);
        }
        
        public string setmapCMD(string msg, string GUID, string RCONpw)
        {
            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " You do not have permission to setmap", GUID, RCONpw);
        }

        public string setgamemodeCMD(string msg, string GUID, string RCONpw)
        {
            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " You do not have permission to setgamemode", GUID, RCONpw);
        }

        public string voteCMD(string msg, string GUID, string RCONpw)
        {
            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " You do not have permission to vote", GUID, RCONpw);
        }

        public string startvoteCMD(string msg, string GUID, string RCONpw)
        {
            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " You do not have permission to startvote", GUID, RCONpw);
        }

        public string statsCMD(string msg, string GUID, string RCONpw)
        {
            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " You do not have permission to stats", GUID, RCONpw);
        }

        public string playerlistCMD(string msg, string GUID, string RCONpw)
        {
            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " You do not have permission to playerlist", GUID, RCONpw);
        }
        
        public string playlistCMD(string msg, string GUID, string RCONpw)
        {
            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " You do not have permission to playlist", GUID, RCONpw);
        }

        public string setadminvarCMD(string msg, string GUID, string RCONpw)
        {
            return this.tellCMD(
                "tell " + this.currentPlayers[GUID].Name + " You do not have permission to setadminvar", GUID, RCONpw);
        }

        public string tellCMD(string msg, string GUID, string RCONpw)
        {
            var msgLoad = string.Empty;
            var origSlotNum = this.currentPlayers[GUID].SlotNumber.ToString();

            if (this.HasPermission(GUID, "tell"))
            {
                var name = msg.Split(' ')[1];

                if (name.Length < 3)
                {
                    msgLoad = "tell " + origSlotNum + " player name too short to make comparison, enter at least 3 characters";
                    return this.ExecuteCommand(msgLoad, RCONpw);
                }

                var foundName = this.PartialPlayerPresent(name);

                if (foundName == string.Empty)
                {
                    msgLoad = "tell " + origSlotNum + " no player name or multiple names found";
                    return this.ExecuteCommand(msgLoad, RCONpw);
                }

                msgLoad = "tell " + this.currentPlayers[foundName].SlotNumber + " ";

                var splitMsg = msg.Split(' ');

                for (var i = 0; i < splitMsg.Length; i++)
                {
                    if (i > 1)
                    {
                        msgLoad += splitMsg[i] + " ";
                    }
                }

                return this.ExecuteCommand(msgLoad, RCONpw);
            }

            msgLoad = "tell " + origSlotNum + " error in tell";

            return this.ExecuteCommand(msgLoad, RCONpw);
        }


        private void SQLDisconnect()
        {
            this.SQLconnection.Close();
        }

        private bool KnownCommand(string cmd)
        {
            return this.commandList.Any(cmdCheck => cmdCheck == cmd);
        }

        private string ExecuteCommand(string cmd, string RCONpw)
        {
            if (!this.gameServer.Connected)
            {
                this.RCONConnect();
            }

            if (this.gameServer.Connected)
            {
                var loginPackage = new byte[RCONpw.Length + 7 + cmd.Length];

                loginPackage[0] = 255;
                loginPackage[1] = 255;
                loginPackage[2] = 255;
                loginPackage[3] = 255;
                loginPackage[4] = 0;

                for (var i = 0; i < RCONpw.Length; i++)
                {
                    loginPackage[5 + i] = (byte)RCONpw[i];
                }

                loginPackage[5 + RCONpw.Length] = 32;

                for (var i = 0; i < cmd.Length; i++)
                {
                    loginPackage[5 + RCONpw.Length + 1 + i] = (byte)cmd[i];
                }

                loginPackage[loginPackage.Length - 1] = 0;

                this.gameServer.Send(loginPackage);

                var response = new byte[255];

                this.gameServer.Receive(response, 0, 255, SocketFlags.None);

                var responseString = new char[response.Length];

                for (var i = 0; i < response.Length - 5; i++)
                {
                    if (response[5 + i] == 10)
                    {
                        break;
                    }

                    responseString[i] = (char)response[5 + i];
                }

                return new string(responseString);
            }

            throw new IOException("connection to RCON fubar");
        }

        public bool getMaps()
        {
            MySqlDataReader dr;
            var hasPermSP = new MySqlCommand("getMaps", this.SQLconnection);
            hasPermSP.CommandType = CommandType.StoredProcedure;
            hasPermSP.Connection.Open();
            dr = hasPermSP.ExecuteReader(CommandBehavior.CloseConnection);

            try
            {
                var messageItemlist = new List<string>();

                while (dr.Read())
                {
                    messageItemlist.Add((string)dr["CommonName"]);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            dr.Close();
            return false;
        }

        public bool getTypes()
        {
            MySqlDataReader dr;
            var hasPermSP = new MySqlCommand("getType", this.SQLconnection);
            hasPermSP.CommandType = CommandType.StoredProcedure;
            hasPermSP.Connection.Open();
            dr = hasPermSP.ExecuteReader(CommandBehavior.CloseConnection);

            try
            {
                var messageItemlist = new List<string>();

                while (dr.Read())
                {
                    messageItemlist.Add((string)dr["playListname"]);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            dr.Close();
            return false;
        }

        public bool getCurrentRotation()
        {
            var hasPermSP = new MySqlCommand("getCurrentRotation", this.SQLconnection);
            hasPermSP.CommandType = CommandType.StoredProcedure;
            hasPermSP.Connection.Open();
            var dr = hasPermSP.ExecuteReader(CommandBehavior.CloseConnection);

            var messageItemlist = new List<string>();
            var messageItemlist2 = new List<string>();
            
            try
            {
                this.gameRotation = new System.Collections.Generic.List<RotationEntry>();
                while (dr.Read())
                {
                    var tempRotatationEntry = new RotationEntry();
                    tempRotatationEntry.m.commonName =(string)dr["mapid"];
                    tempRotatationEntry.t.typeName = (string)dr["gametypeid"];
                    //messageItemlist.Add((string)dr["mapid"]);
                    //messageItemlist2.Add((string)dr["gametypeid"]);

                    this.gameRotation.Add(new 
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            dr.Close();
            return false;
        }

        public bool HasPermission(string GUID, string cmd)
        {
            if (GUID == "-1")
            {
                return true;
            }

            var hasPermSP = new MySqlCommand("HasPermission", this.SQLconnection);
            hasPermSP.CommandType = CommandType.StoredProcedure;
            hasPermSP.Parameters.Add(new MySqlParameter("GUIDvar", GUID));
            hasPermSP.Parameters.Add(new MySqlParameter("CMDvar", cmd));
            hasPermSP.Connection.Open();
            var dr = hasPermSP.ExecuteReader(CommandBehavior.CloseConnection);

            var messageItemlist = new List<string>();

            while (dr.Read())
            {
                messageItemlist.Add((string)dr["commandName"]);
            }

            dr.Close();

            return messageItemlist.Count == 1 && messageItemlist[0] == cmd;
        }
    }
}
