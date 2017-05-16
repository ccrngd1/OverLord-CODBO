using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace RCONtest
{
    public partial class Form1 : Form
    {
        private string lastLine = "";
        private Dictionary<string, Player> currentPlayers = new Dictionary<string,Player>();
        Server instance1;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            instance1 = new Server("173.199.91.92", 3074, "951004", "66.55.146.25", "c951004_phpFusion", "c951004", "GamersontheFringe11", "http://logs.gameservers.com/173.199.91.92:3074/91da5cf3-308a-4beb-b76f-89dc7a4eb889");
            instance1.RCONConnect();

            // this test uses my GUID and the server pw to say 'hi ho'
            // var response = instance1.sayCMD("hi ho", "9743419", "951004");
        }

        private void Form1_Load2(object sender, EventArgs e)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try
            {
                s.Connect("173.199.91.92", 3074);

                if (s.Connected)
                {
                    Console.Write("yay");

                    string pw = "951004";
                    string cmd = "say hi";
                    var loginPackage = new byte[pw.Length + 7 + cmd.Length];

                    loginPackage[0] = 255;
                    loginPackage[1] = 255;
                    loginPackage[2] = 255;
                    loginPackage[3] = 255;
                    loginPackage[4] = 0;

                    for (int i = 0; i < pw.Length; i++)
                    {
                        loginPackage[5 + i] = (byte)pw[i];
                    }

                    loginPackage[5 + pw.Length] = 32;

                    for (int i = 0; i < cmd.Length; i++)
                    {
                        loginPackage[5 + pw.Length + 1 + i] = (byte)cmd[i];
                    }

                    loginPackage[loginPackage.Length - 1] = 0;

                    s.Send(loginPackage);

                    byte[] response = new byte[255];

                    s.Receive(response, 0, 255, SocketFlags.None);

                    char[] responseString = new char[response.Length];
                    string str = "";
                    for (int i = 0; i < response.Length - 5; i++)
                    {
                        if (response[5 + i] == 10)
                            break;

                        responseString[i] = (char)response[5 + i];
                    }
                    str = new string(responseString);
                }
            }
            catch (Exception ex)
            {
                Console.Write("crap");
            }

            try
            {
                // used to build entire input
                StringBuilder sb = new StringBuilder();

                // used on each read operation
                byte[] buf = new byte[8192];

                // prepare the web page we will be asking for
                HttpWebRequest request = (HttpWebRequest)
                    WebRequest.Create("http://logs.gameservers.com/173.199.91.92:3074/91da5cf3-308a-4beb-b76f-89dc7a4eb889");

                // execute the request
                HttpWebResponse response = (HttpWebResponse)
                    request.GetResponse();

                // we will read data via the response stream
                Stream resStream = response.GetResponseStream();

                string tempString = null;
                int count = 0;

                do
                {
                    // fill the buffer with data
                    count = resStream.Read(buf, 0, buf.Length);

                    // make sure we read some data
                    if (count != 0)
                    {
                        // translate from bytes to ASCII text
                        tempString = Encoding.ASCII.GetString(buf, 0, count);

                        // continue building the string
                        sb.Append(tempString);
                    }
                }
                while (count > 0); // any more data to read?

                string[] splits = sb.ToString().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < splits.Length; i++)
                {
                    string[] semiColSplit = splits[i].Split(';');
                    if (semiColSplit.Length > 1)
                    {
                        var t = semiColSplit[0].IndexOf(' ');
                        switch (semiColSplit[0].Substring(t + 1, semiColSplit[0].Length - t - 1))
                        {
                            //pattern:
                            //Time&Kill; GUID-victim, slotID-victim, team-victim, name-victim, GUID-killer, slotID-killer, team-killer, name-killer, weapon w/ mod, dmg, object that killed, location hit
                            case "K":
                                if (semiColSplit[3] == semiColSplit[7]) //team kill
                                {
                                    //ToDo: add this code
                                    //if(kill != via killstreak)
                                    currentPlayers[semiColSplit[5]].TeamKills++;

                                    //this is where we need to check player status to see if we got an owner/leader
                                    //we also want to check how the TK happened, if it was napalm/heuy, hind, etc, we should prolly let it go
                                    //lastly, if the dude is 100-1, lets assume he didn't do it because he's a dick or a noob

                                    if (currentPlayers[semiColSplit[5]].Status == PlayerStatus.pub
                                        || currentPlayers[semiColSplit[5]].Status == PlayerStatus.friend
                                        || currentPlayers[semiColSplit[5]].Status == PlayerStatus.invalid
                                        || currentPlayers[semiColSplit[5]].Status == PlayerStatus.member)
                                    {
                                        //this is a random limit, no fewer than 3
                                        if (currentPlayers[semiColSplit[5]].TeamKills > 4)
                                        {
                                            //so the person is over the hard limit, let's see how well he is doing
                                            //if he has more kills than deaths, thats a good start
                                            //he should also have a high number of kills or we can assume he isn't doing so well, 15 seems like a good number
                                            if (currentPlayers[semiColSplit[5]].Kills > currentPlayers[semiColSplit[5]].Deaths
                                                && currentPlayers[semiColSplit[5]].Kills > 15
                                                && currentPlayers[semiColSplit[5]].Kills / currentPlayers[semiColSplit[5]].TeamKills > 3)
                                            {

                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    if (currentPlayers.ContainsKey(semiColSplit[5]))
                                    {
                                        currentPlayers[semiColSplit[5]].Kills++;
                                    }
                                }

                                if (currentPlayers.ContainsKey(semiColSplit[1]))
                                {
                                    currentPlayers[semiColSplit[1]].Deaths++;
                                }

                                break;

                            //Damage
                            case "D":
                                break;

                            case "J":
                                if (!currentPlayers.ContainsKey(semiColSplit[1]) && semiColSplit[3].ToLower() != "[3arc]democlient")
                                {
                                    currentPlayers.Add(semiColSplit[1], new Player(semiColSplit[3], semiColSplit[1], Convert.ToInt32(semiColSplit[2])));
                                }
                                break;

                            case "Q":
                                if (currentPlayers.ContainsKey(semiColSplit[1]))
                                {
                                    currentPlayers.Remove(semiColSplit[1]);
                                }
                                break;

                            //format:
                            //[0]time&say, [1]GUID, [2]slot#, [3]name, [4]msg
                            case "say":
                                if (semiColSplit[4].Contains('!'))
                                {
                                    var a = 0;
                                }


                                if (semiColSplit[4][0] == '!' || semiColSplit[4][1] == '!')
                                {
                                    //ParsePlayerCommand(semiColSplit[1], semiColSplit[4]);
                                }
                                break;

                            case "sayteam":
                                break;

                            case "weapon":
                                break;

                            default:
                                break;
                        }
                    }
                }
                lastLine = splits[splits.Length - 1];
            }
            catch (Exception ex)
            {

            }
        }

        private void tabControl2_Enter(object sender, EventArgs e)
        {
            this.instance1.getCurrentRotation();
            this.instance1.getMaps();
            this.instance1.getTypes();


        }

    }
}
