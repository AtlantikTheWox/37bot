using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;
using Discord.Rest;
using Microsoft.Extensions.Configuration.Json;
using botof37s;
using botof37s.services;
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

namespace botof37s.Modules
{
    public class Twitchcommand : ModuleBase
    {
        public IConfiguration _config { get; set; }
        public DiscordSocketClient _client { get; set; }
        public CommandHandler handler { get; set; }
        //file structure: 
        //twitch: Name = Twitch ID + .37, Content = Discord UID
        //twitchlink: Name = Twitch username + .37, Line 1 = discord UID, Line 2 = key
        [Command("twitch")]
        [Summary("Manages the Twitch account connection")]
        [Name("<:twitch:817122569554886698> twitch <link|remove> <Twitch username>")]
        [Remarks("all")]
        public async Task TwitchCommand(string argument = null, [Remainder]string username = null)
        {
            switch (argument)
            {
                case "link":
                    DirectoryInfo di = new DirectoryInfo("twitch");
                    foreach (FileInfo file in di.GetFiles())
                    {
                        if (File.ReadAllText($"twitch/{file.Name}") == Context.User.Id.ToString())
                        {
                            await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> You have already linked an account. To link a new account you have to remove your current account first.");
                            return;
                        }
                    }
                    if(username == null)
                    {
                        await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> You need to provide a username!");
                        return;
                    }
                    if (File.Exists($"twitch/{username}.37")||File.Exists($"twitclink/{username}.37"))
                    {
                        var guildList = _client.Guilds;
                        string admin = "";
                        foreach (SocketGuild guild in guildList)
                        {
                            foreach (SocketUser user in guild.Users)
                            {
                                if (Convert.ToString(user.Id) == _config["AdminUserID"])
                                {
                                    admin = user.Username + "#" + user.Discriminator;
                                    goto stop;
                                }
                            }
                        }
                    stop:;
                        await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> The Twitch account you provided is already linked to or awaiting verification with another Discord account. If you think this is a mistake contact {admin}");
                        return;
                    }
                    DirectoryInfo di2 = new DirectoryInfo("twitchlink");
                    foreach (FileInfo file2 in di2.GetFiles())
                    {
                        if (File.ReadAllLines($"twitchlink/{file2.Name}")[0].Replace(".37", "") == Context.User.Id.ToString()) 
                        {
                            file2.Delete();
                            goto skip;
                        }
                    }
                skip:;
                    int key = new Random().Next(111111, 999999);
                    if (Convert.ToString(Context.Channel.GetType()) != "Discord.WebSocket.SocketDMChannel")
                    {
                        await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Check your DMs");
                    }
                    string channel = _config["Broadcaster"];
                    await Context.User.SendMessageAsync($"<@{Context.User.Id}> Please make sure you're logged into your Twitch account, go to <https://twitch.tv/{channel}> and enter '!37 verify {key}' in chat. Your verification key will expire in 3 minutes");
                    File.WriteAllText($"twitchlink/{username}.37", $"{Context.User.Id}\n{key}");
                    handler.FileExpiryAsync($"twitchlink/{username}");


                    return;
                case "remove":
                    DirectoryInfo di3 = new DirectoryInfo("twitch");
                    foreach (FileInfo file in di3.GetFiles())
                    {
                        if (File.ReadAllText($"twitch/{file.Name}") == Context.User.Id.ToString())
                        {
                            file.Delete();
                            await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Twitch account unlinked successfully");
                        }
                    }
                    return;
                default:
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Invalid argument(s)");
                    return;
            }
        }
    }
}