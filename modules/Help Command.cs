



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


    public class Helpcommand : ModuleBase
    {
        public CommandService Cservice { get; set; } 
        public DiscordSocketClient _client { get; set; }
        public IConfiguration _config { get; set; }
            
        [Command("help")]
        [Alias("manual")]
        [Name("❓ help")]
        [Summary("Displays this window")]
        public async Task Help()
        {
            
            List<CommandInfo> commands = Cservice.Commands.ToList();
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithAuthor("37 Help", "https://cdn.discordapp.com/app-icons/737060692527415466/c64109fbdff1a1f6dfd7515eaec5198d.png?size=512", "https://bit.ly/37status");
            foreach (CommandInfo command in commands)
            {
                // Get the command Summary attribute information
                string embedFieldText = command.Summary ?? "No description available\n";
                embedBuilder.AddField(command.Name, embedFieldText);
            }
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
            Colorpicker colorpicker = new Colorpicker();
            embedBuilder.WithColor(colorpicker.Pick());
            embedBuilder.WithFooter($"If you encounter any issues contact {admin}", "https://cdn.discordapp.com/emojis/734132648800419880.png");
            await ReplyAsync(null, false, embedBuilder.Build());
        }
    }
}