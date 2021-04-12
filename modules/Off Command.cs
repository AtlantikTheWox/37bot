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
    


    public class Offcommand : ModuleBase
    {
        public TwitchClient twitchclient { get; set; }
        public IConfiguration _config { get; set; }
        public DiscordSocketClient _client { get; set; }

        [Command("off")]
        [Summary("Well... it turns of the bot")]
        [Name("⚡ off")]
        [RequireOwner]
        [Remarks("owner")]
        public async Task OffCommand()
        {
            twitchclient.SendMessage(_config["Broadcaster"], "I'm shutting down and hope I'll be back soon");
            await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Goodbye <:37a:712802388955758614><:37b:712802398854316052><:37c:712802406173245460>");
            twitchclient.Disconnect();
            await _client.StopAsync();
            Environment.Exit(37);
        }
    }
}