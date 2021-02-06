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

namespace botof37s.Modules
{


    public class Offcommand : ModuleBase
    {

        [Command("off")]
        [Summary("Well... it turns of the bot")]
        [Name("⚡ off")]
        [RequireOwner]
        [Remarks("owner")]
        public async Task OffCommand([Remainder] string args = null)
        {
            await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Goodbye <:37a:712802388955758614><:37b:712802398854316052><:37c:712802406173245460>");
            Environment.Exit(37);
        }
    }
}