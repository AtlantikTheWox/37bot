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
using botof37s.services;
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
        [Remarks("all")]
        public async Task Help()
        {
            
            List<CommandInfo> commands = Cservice.Commands.ToList();
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithAuthor("37 Help", "https://cdn.discordapp.com/app-icons/737060692527415466/c64109fbdff1a1f6dfd7515eaec5198d.png?size=512", "https://bit.ly/37status");
            foreach (CommandInfo command in commands)
            {
                // Get the command Summary attribute information
                if (command.Remarks == "all") 
                {
                    string embedFieldText = command.Summary ?? "No description available\n";
                    embedBuilder.AddField(command.Name, embedFieldText,true);
                }
            }
            Authorisationcheck check = new Authorisationcheck();
            if (check.Check(Context.User.Id,_config))
            {
                embedBuilder.AddField("Admin Commands", "You have access to these command because you're authorized", false);
                foreach(CommandInfo command in commands)
                {
                    if (command.Remarks == "authorized")
                    {
                        string embedFieldText = command.Summary ?? "No description available\n";
                        embedBuilder.AddField(command.Name, embedFieldText, true);
                    }
                }
            }
            if(Context.User.Id.ToString() == _config["AdminUserID"])
            {
                embedBuilder.AddField("Owner Commands", "You have access to these command because you're the bot's owner", false);
                foreach (CommandInfo command in commands)
                {
                    if (command.Remarks == "owner")
                    {
                        string embedFieldText = command.Summary ?? "No description available\n";
                        embedBuilder.AddField(command.Name, embedFieldText, true);
                    }
                }
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
            embedBuilder.WithFooter($"If you encounter any issues contact {admin}", "https://cdn.discordapp.com/avatars/329650083819814913/33b46ac7c4bfa97c6df65b108fd8c008.png?size=512");
            await ReplyAsync(null, false, embedBuilder.Build());
        }
    }
}