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
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Discord.Rest;
using Microsoft.Extensions.Configuration.Json;
using botof37s;
using colorpicker;

namespace botof37s.Modules
{


    public class Authorisationcommand : ModuleBase
    {
        

        [Command("authorized")]
        [Summary("Manages the list of authorized people")]
        [Name("👮 authorized <add|remove|list> (<user id>)")]
        [Remarks("owner")]
        [RequireOwner]
        public async Task AuthorizeCommand(string option = null, [Remainder] long args = 0)
        {
            switch (option)
            {
                case "add":
                    if (args == 0)
                    {
                        await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> You need to provide a user id");
                        break;
                    }
                    if(File.Exists($"authorized/{args}.37"))
                    {
                        await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> This user is already authorized");
                        break;
                    }
                    File.Create($"authorized/{args}.37");
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Success");
                    break;
                case "remove":
                    if (args == 0)
                    {
                        await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> You need to provide a user id");
                        break;
                    }
                    if (!File.Exists($"authorized/{args}.37"))
                    {
                        await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> This user isn't authorized");
                        break;
                    }
                    File.Delete($"authorized/{args}.37");
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Success");
                    break;
                case "list":
                    DirectoryInfo di = new DirectoryInfo("authorized");
                    EmbedBuilder builder = new EmbedBuilder();
                    string list = "";
                    foreach(FileInfo file in di.GetFiles())
                    {
                        string id = file.Name.Replace(".37", "");
                        string username = "Unknown user";
                        DiscordSocketClient _client = (DiscordSocketClient)Context.Client;
                        var guildList = _client.Guilds;
                        foreach (SocketGuild guild in guildList)
                        {
                            foreach (SocketUser user in guild.Users)
                            {
                                if (Convert.ToString(user.Id) == id)
                                {
                                    username = user.Username + "#" + user.Discriminator;
                                    goto stop;
                                }
                            }
                        }
                    stop:;
                        list = list + "\n" + id + " - " + username;    
                    }
                    if(list == "")
                    {
                        list = "No people have been authorized yet";
                    }
                    builder.WithAuthor("37 Authorisation list", "https://cdn.discordapp.com/app-icons/737060692527415466/c64109fbdff1a1f6dfd7515eaec5198d.png?size=512", "https://bit.ly/37status");
                    builder.WithDescription(list);
                    builder.WithFooter($"No responsibility is accepted for the accuracy of this information.", "https://cdn.discordapp.com/emojis/734132648800419880.png");
                    Colorpicker picker = new Colorpicker();
                    builder.WithColor(picker.Pick());
                    await Context.Channel.SendMessageAsync(null, false, builder.Build());
                    break;
                default:
                    return;
            }
        }
    }
}