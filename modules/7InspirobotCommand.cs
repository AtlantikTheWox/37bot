using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;
using Discord.Rest;
using Microsoft.Extensions.Configuration.Json;
using botof37s;
using colorpicker;
using System.Net;

namespace botof37s.Modules
{


    public class Inspirobotcommand : ModuleBase
    {
        public IConfiguration _config { get; set; }
        [Command("inspirobot")]
        [Alias("inspire")]
        [Name("🤖 inspirobot")]
        [Summary("Gets a motivational quote from inspirobot")]
        [Remarks("all")]
        public async Task InspirobotCommand()
        {
            HttpClient client = new HttpClient();
            var piclink = await (await client.GetAsync("https://inspirobot.me/api?generate=true")).Content.ReadAsStringAsync();
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithAuthor("Here's your inspiration:", "https://inspirobot.me/website/images/inspirobot-dark-green.png",piclink);
            builder.WithImageUrl(piclink);
            var admin = await Context.Client.GetUserAsync(ulong.Parse(_config["AdminUserID"]));
            builder.WithFooter("Quote provided by inspirobot.me", admin.GetAvatarUrl());
            Colorpicker colorpicker = new Colorpicker();
            builder.WithColor(colorpicker.Pick());
            await ReplyAsync(null,false,builder.Build());
        }
    }
}