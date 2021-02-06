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


    public class Colorcommand : ModuleBase
    {

        [Command("color")]
        [Alias("colour")]
        [Summary("Adds a decimal color value to the list of embed colors")]
        [Name("🎨 color <Decimal color>")]
        [RequireOwner]
        [Remarks("owner")]
        public async Task ColorCommand([Remainder] string args = null)
        {
            if (args == null)
            {
                await Context.Channel.SendMessageAsync("Missing argument. You need to provide a decimal color value");
                return;
            }    
            try { long.Parse(args); }
            catch 
            { 
                await Context.Channel.SendMessageAsync("Invalid argument. You need to provide a decimal color value");
                return;
            }
            if (!(0 <= long.Parse(args)) || !(long.Parse(args) <= 16777215))
            {
                await Context.Channel.SendMessageAsync("Invalid argument. You need to provide a decimal color value between 0 and 16777215");
                return;
            }
            if (File.Exists("db/additionalcolors.37"))
                args = "\n" + args;
            File.WriteAllText("db/additionalcolors.37", args);
            await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Success!");
        }
    }
}