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


    public class Activitycommand : ModuleBase
    {

        [Command("activity")]
        [Alias("status")]
        [Summary("Sets the activity of the bot")]
        [Name("💬 activity <reset|playing|listening|watching>")]
        [Remarks("authorized")]
        public async Task ActivitycommandAsync(string type = "reset", [Remainder] string activity = null)
        {
            await Context.Channel.SendMessageAsync($"Type: \"{type}\" activity: \"{activity}\"");
        }
    }
}