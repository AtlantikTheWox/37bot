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
            }
            else if (e.ChatMessage.Message.ToString().StartsWith("!37 "))
            {
                
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
