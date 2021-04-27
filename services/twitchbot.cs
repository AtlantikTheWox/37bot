using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
using botof37s.Modules;

namespace botof37s.services
{
    class Twitchbot
    {
        IConfiguration config;
        TwitchClient twitchclient;
        DiscordSocketClient _client;
        public Twitchbot(TwitchClient tclient, IConfiguration conf, DiscordSocketClient client)
        {
            twitchclient = tclient;
            config = conf;
            _client = client;
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
                    twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} To claim 37s on Twitch, please go to Discord and use \"/37 twitch link {e.ChatMessage.Username}\" to link your accounts first.");
                    return;
                }
                else
                {
                    DateTime last37 = new DateTime();
                    IConfiguration _config;

                    var _builder = new ConfigurationBuilder().
                    SetBasePath(AppContext.BaseDirectory).
                    AddJsonFile(path: "config.json");

                    _config = _builder.Build();
                    if (File.Exists("db/lastmessage.37"))
                    {
                        last37 = Convert.ToDateTime(File.ReadAllText("db/lastmessage.37"));
                    }
                    TimeSpan ts = DateTime.UtcNow - last37;
                    if (ts.TotalMinutes >= Int32.Parse(_config["Frequency"]))
                    {
                        ulong uid = ulong.Parse(File.ReadAllText($"twitch/{e.ChatMessage.UserId}.37"));
                        int personalcount = 0;
                        int counter = 0;
                        if (File.Exists($"leaderboard/{uid}.37"))
                        {
                            personalcount = Int32.Parse(File.ReadAllText($"leaderboard/{uid}.37"));
                        }
                        if (File.Exists("db/counter.37"))
                        {
                            counter = int.Parse(File.ReadAllText("db/counter.37"));
                        }
                        File.WriteAllText("db/lastmessage.37", DateTime.UtcNow.ToString());
                        File.WriteAllText($"leaderboard/{uid}.37", (personalcount + 1).ToString());
                        File.WriteAllText("db/last37uname.37", $"{e.ChatMessage.Username}\nt");
                        File.WriteAllText("db/counter.37", (counter + 1).ToString());

                        Cooldown cooldown = new Cooldown();
                        cooldown.CooldownAsync(Int32.Parse(_config["Frequency"]) * 60 * 1000, _client);
                        var replies = new List<string>
                        {
                             $"@{e.ChatMessage.Username} Coming right up!",
                             $"@{e.ChatMessage.Username} As you wish!",
                             $"@{e.ChatMessage.Username} I cant believe its not spam!",
                             $"@{e.ChatMessage.Username} Ugh, fine!"
                        };
                        var answer = replies[new Random().Next(replies.Count)];
                        twitchclient.SendMessage(e.ChatMessage.Channel,answer);
                    }
                    else
                    {
                        string last37uname = "[REDACTED]";
                        if (File.Exists("db/last37uname.37"))
                        {
                            last37uname = File.ReadAllLines("db/last37uname.37")[0];
                            if (File.ReadAllLines("db/last37uname.37")[1] == "d")
                            {
                                last37uname += " on Discord";
                            }
                            
                        }
                    
                        twitchclient.SendMessage(e.ChatMessage.Channel, $"I'm sorry @{e.ChatMessage.Username}, but you will have to wait another {Math.Floor(Int32.Parse(_config["Frequency"]) - ts.TotalMinutes)} minutes and {60 - ts.Seconds} seconds. The last 37 was claimed by {last37uname}.");
                    }
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
                        twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} This account is currently not pending verification. If you are trying to verify your account please check if you have made a typo.");
                    }

                }
            }
            else if (e.ChatMessage.Message.Contains("37 "))
            {
                int rnd = new Random().Next(0, 7);
                if (rnd == 0) twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} Noice");
                if (rnd == 1) twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} You called?");
                if (rnd == 2) twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} Harry says \"Fuck sake\"");
                if (rnd == 3) twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} I see, so that's how it's gonna be huh?");
                if (rnd == 4) twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} I'm just gonna silently judge you");
                if (rnd == 5) twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} Wow, you think this is funny");
                if (rnd == 6) twitchclient.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username} Haha, funy failed test numba");
            }
            
        }
    }
}
