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
using colorpicker;

namespace botof37s.Modules
{


    public class Count : ModuleBase
    {
        [Command("count")]
        [Alias("counter")]
        [Summary("Returns a count of all 37s claimed so far")]
        [Name("🔢 count")]
        public async Task CountAsync()
        {
            int counter = 0;
            if (File.Exists("db/counter.37"))
            {
                counter = int.Parse(File.ReadAllText("db/counter.37"));
            }
            EmbedBuilder builder = new EmbedBuilder();
            IConfiguration _config;

            var _configbuilder = new ConfigurationBuilder().
            SetBasePath(AppContext.BaseDirectory).
            AddJsonFile(path: "config.json");

            _config = _configbuilder.Build();
            var _client = (DiscordSocketClient)Context.Client;
            var guildList = _client.Guilds;
            string admin = "";
            foreach (SocketGuild guild in guildList)
            {
                foreach (SocketUser user in guild.Users)
                {
                    if (Convert.ToString(user.Id) == _config["AdminUserID"])
                    {
                        admin = user.Username +"#" + user.Discriminator;
                        goto stop;
                    }
                }
            }
            stop:;
            builder.WithAuthor("37 Counter", "https://cdn.discordapp.com/app-icons/737060692527415466/c64109fbdff1a1f6dfd7515eaec5198d.png?size=512", "https://bit.ly/37status");
            builder.AddField($"This is how many 37s have been claimed:", $"{counter}", true);
            builder.WithFooter($"If you encounter any issues contact {admin}", "https://cdn.discordapp.com/emojis/734132648800419880.png");
            Colorpicker picker = new Colorpicker();
            builder.WithColor((uint)picker.Pick());
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }
    }
}