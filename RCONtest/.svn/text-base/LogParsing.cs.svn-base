using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace RCONtest
{
    class LogParsing
    {
        private string lastLine;

        private double lastTime=-1;

        private string serverLog;

        private Server owningServer;

        private Thread parseThread;

        private static bool run = true;

        public LogParsing(string logAddress, Server parent)
        {
            this.serverLog = logAddress;
            this.owningServer = parent;

            //this.owningServer.ParsePlayerCommand("-1", "!kick ccr douchebag");
            //this.owningServer.ParsePlayerCommand("-1", "!say stop being a douchebag");
        }

        public void Parse()
        {
            this.parseThread = new Thread((ThreadStart)this.ParseMutliThreaded);
            this.parseThread.Start();
        }

        public void Shutdown()
        {
            this.parseThread.Join();
            this.parseThread.Abort();
        }

        public void ParseMutliThreaded()
        {
            while(run)
            {
                try
                {
                    // used to build entire input
                    var sb = new StringBuilder();

                    // used on each read operation
                    var buf = new byte[8192];

                    // prepare the web page we will be asking for
                    var request = (HttpWebRequest)WebRequest.Create(this.serverLog);
                    request.ServicePoint.ConnectionLimit = 50;

                    // execute the request
                    var response = (HttpWebResponse)request.GetResponse();

                    // we will read data via the response stream
                    var resStream = response.GetResponseStream();

                    var count = 0;

                    do
                    {
                        // fill the buffer with data
                        if (resStream != null)
                        {
                            count = resStream.Read(buf, 0, buf.Length);
                        }

                        // make sure we read some data
                        if (count == 0)
                        {
                            break;
                        }

                        // translate from bytes to ASCII text
                        var tempString = Encoding.ASCII.GetString(buf, 0, count);

                        // continue building the string
                        sb.Append(tempString);
                    }
                    while (count > 0); // any more data to read?

                    var splits = sb.ToString().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    var moreLog = false;

                    for (var i = 0; i < splits.Length; i++)
                    {
                        var semiColSplit = splits[i].Split(';');

                        var currentTime = Convert.ToDouble(semiColSplit[0].Split(' ')[0]);

                        if (currentTime < this.lastTime)
                        {
                            continue;
                        }

                        if (currentTime == this.lastTime && moreLog == false)
                        {
                            if (splits[i] == this.lastLine)
                            {
                                moreLog = true;
                            }

                            continue;
                        }

                        if (semiColSplit.Length <= 1)
                        {
                            continue;
                        }

                        var t = semiColSplit[0].IndexOf(' ');
                        switch (semiColSplit[0].Substring(t + 1, semiColSplit[0].Length - t - 1))
                        {
                                // pattern:
                                // [0]Time&Kill; [1]GUID-victim, [2]slotID-victim, [3]team-victim, [4]name-victim, [5]GUID-killer, [6]slotID-killer, [7]team-killer, [8]name-killer, [9]weapon w/ mod, [10]dmg, [11]object that killed, [12]location hit
                            case "K":
                                // team kill
                                if (semiColSplit[3] == semiColSplit[7])
                                {
                                    // ToDo: add this code
                                    // if(kill != via killstreak)
                                    // this.owningServer.currentPlayers[semiColSplit[5]].TeamKills++;
                                    this.owningServer.AlterPlayerTeamKillCount(semiColSplit[5], 1);
                                    
                                    // this is where we need to check player status to see if we got an owner/leader
                                    // we also want to check how the TK happened, if it was napalm/heuy, hind, etc, we should prolly let it go
                                    // lastly, if the dude is 100-1, lets assume he didn't do it because he's a dick or a noob
                                    // if (this.owningServer.currentPlayers[semiColSplit[5]].Status == PlayerStatus.pub ||
                                    //    this.owningServer.currentPlayers[semiColSplit[5]].Status == PlayerStatus.friend ||
                                    //    this.owningServer.currentPlayers[semiColSplit[5]].Status == PlayerStatus.invalid ||
                                    //    this.owningServer.currentPlayers[semiColSplit[5]].Status == PlayerStatus.member)
                                    if (this.owningServer.PlayerStatusCheck(semiColSplit[5]) == PlayerStatus.pub ||
                                        this.owningServer.PlayerStatusCheck(semiColSplit[5]) == PlayerStatus.friend ||
                                        this.owningServer.PlayerStatusCheck(semiColSplit[5]) == PlayerStatus.invalid ||
                                        this.owningServer.PlayerStatusCheck(semiColSplit[5]) == PlayerStatus.member)
                                    {
                                        // this is a random limit, no fewer than 3
                                        // if (this.owningServer.currentPlayers[semiColSplit[5]].TeamKills > 4)
                                        if (this.owningServer.AlterPlayerTeamKillCount(semiColSplit[5], 0) > 4)
                                        {
                                            // so the person is over the hard limit, let's see how well he is doing
                                            // if he has more kills than deaths, thats a good start
                                            // he should also have a high number of kills or we can assume he isn't doing so well, 15 seems like a good number
                                            // ratio for kicking, for example if 15 kills then can't have more than 5 tks
                                            // if (this.owningServer.currentPlayers[semiColSplit[5]].Kills >
                                            //    this.owningServer.currentPlayers[semiColSplit[5]].Deaths &&
                                            //    this.owningServer.currentPlayers[semiColSplit[5]].Kills > 15 &&
                                            //    this.owningServer.currentPlayers[semiColSplit[5]].Kills /
                                            //    this.owningServer.currentPlayers[semiColSplit[5]].TeamKills > 3)
                                            if (this.owningServer.AlterPlayerKillCount(semiColSplit[5], 0) >
                                                this.owningServer.AlterPlayerDeathCount(semiColSplit[5], 0) &&
                                                this.owningServer.AlterPlayerKillCount(semiColSplit[5], 0) >= 15 &&
                                                this.owningServer.AlterPlayerKillCount(semiColSplit[5], 0) /
                                                this.owningServer.AlterPlayerTeamKillCount(semiColSplit[5], 0) <= 3) 
                                            {
                                                var a = 0;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    // if (this.owningServer.currentPlayers.ContainsKey(semiColSplit[5]))
                                    // {
                                        // this.owningServer.currentPlayers[semiColSplit[5]].Kills++;
                                    // }
                                    this.owningServer.AlterPlayerKillCount(semiColSplit[5], 1);
                                }

                                // if (this.owningServer.currentPlayers.ContainsKey(semiColSplit[1]))
                                // {
                                //    this.owningServer.currentPlayers[semiColSplit[1]].Deaths++;
                                // }
                                this.owningServer.AlterPlayerDeathCount(semiColSplit[1], 1);

                                break;

                                // Damage
                            case "D":
                                break;

                            case "J":
                                // if (!this.owningServer.currentPlayers.ContainsKey(semiColSplit[1]) &&
                                //    semiColSplit[3].ToLower() != "[3arc]democlient")
                                // {
                                //    this.owningServer.currentPlayers.Add(
                                //        semiColSplit[1],
                                //        new Player(semiColSplit[3], semiColSplit[1], Convert.ToInt32(semiColSplit[2])));
                                // }
                                this.owningServer.PlayerJoin(semiColSplit[3], semiColSplit[1], semiColSplit[2]);
                                break;

                            case "Q":
                                // if (this.owningServer.currentPlayers.ContainsKey(semiColSplit[1]))
                                // {
                                //    this.owningServer.currentPlayers.Remove(semiColSplit[1]);
                                // }
                                this.owningServer.PlayerQuit(semiColSplit[1]);
                                break;

                                // format:
                                // [0]time&say, [1]GUID, [2]slot#, [3]name, [4]msg
                            case "say":
                                // it seems like a character is in front of the !, I don't believe it's always there or it's something I'm doing
                                // so to be safe, we check the first and second spaces for the command indicator
                                if (semiColSplit[4][0] == '!' || semiColSplit[4][1] == '!')
                                {
                                    this.owningServer.ParsePlayerCommand(semiColSplit[1], semiColSplit[4]);
                                }

                                break;

                            case "sayteam":
                                if (semiColSplit[4][0] == '!' || semiColSplit[4][1] == '!')
                                {
                                    this.owningServer.ParsePlayerCommand(semiColSplit[1], semiColSplit[4]);
                                }

                                break;

                            case "weapon":
                                break;

                            default:
                                break;
                        }
                    }

                    this.lastLine = splits[splits.Length - 1];
                    this.lastTime = Convert.ToDouble(this.lastLine.Split(' ')[0]);

                    Thread.Sleep(3000);
                }
                catch (Exception ex)
                {
                    var a = 0;
                }
            }
        }
    }
}
