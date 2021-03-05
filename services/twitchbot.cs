using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Enums;
using Microsoft.Extensions.Configuration;

namespace botof37s.services
{
    class Twitchbot
    {
        IConfiguration config;
        TwitchClient twitchclient;
        public Twitchbot(TwitchClient client, IConfiguration conf)
        {
            twitchclient = client;
            config = conf;
            Main();
        }
            

        void Main()
        {
            twitchclient.OnLog += TwitchLog;
            twitchclient.OnMessageReceived += TwitchMessageHandler;
        }
        private void TwitchLog(object sender, OnLogArgs e)

        {
            Console.WriteLine($"{e.DateTime:HH:mm:ss} TwitchBot - {e.Data}");
        }
        private void TwitchMessageHandler(object sender, OnMessageReceivedArgs e)
        {
            if(twitchclient.TwitchUsername == e.ChatMessage.Username)
            {
                return;
            }
            if(e.ChatMessage.Message == "!37")
            {
                if (!File.Exists($"twitch/{e.ChatMessage.UserId}.37"))
                {
                    twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} To claim 37s on Twitch, please go to Discord and use \"/37 twitch link <Your Twitch username>\" to link your accounts first.");
                    return;
                }
                else
                {
                    //todo
                }
            }
            else if (e.ChatMessage.Message.ToString().StartsWith("!37 "))
            {
                string messig = e.ChatMessage.Message.Remove(0, 4);
                if (messig.StartsWith("verify"))
                {
                    if(messig.Remove(0,6) == "")
                    {
                        twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} You need to provide a verification Key!");
                        return;
                    }
                    else if (File.Exists($"twitchlink/{e.ChatMessage.Username}.37"))
                    {
                        if(messig.Remove(0,7) == File.ReadAllLines($"twitchlink/{e.ChatMessage.Username}.37")[1])
                        {
                            File.WriteAllText($"twitch/{e.ChatMessage.UserId}.37", File.ReadAllLines($"twitchlink/{e.ChatMessage.Username}.37")[0]);
                            File.Delete($"twitchlink/{e.ChatMessage.Username}.37");
                            twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} Verification successful!");
                            return;
                        }
                        else
                        {
                            twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} Invalid verification key! Please try again.");
                            return;
                        }
                    }
                    else if (File.Exists($"twitchlink/{e.ChatMessage.Username}_expired.37"))
                    {
                        twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} I'm sorry, but your verification request has expired. Please try again.");
                        File.Delete($"twitchlink/{e.ChatMessage.Username}_expired.37");
                        return;
                    }
                    else
                    {
                        twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} This account is currently not pending verification. If you are trying to verify your account please check if you made a typo.");
                    }

                }
            }
            else if (e.ChatMessage.Message.Contains("37"))
            {
                int rnd = new Random().Next(0, 6);
                if (rnd == 0) twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} Noice");
                if (rnd == 1) twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} You called?");
                if (rnd == 2) twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} Harry says \"Fuck sake\"");
                if (rnd == 3) twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} I see, so that's how it's gonna be huh?");
                if (rnd == 4) twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} I'm just gonna silently judge you");
                if (rnd == 5) twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} Wow, you think this is funny");
            }
            
        }
    }
}
