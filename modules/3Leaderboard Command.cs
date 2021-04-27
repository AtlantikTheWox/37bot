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


    public class Leaderboardcommand : ModuleBase
    {

        [Command("ranking")]
        [Alias("leaderboard")]
        [Summary("Displays the leaderboard")]
        [Name("📑 ranking")]
        [Remarks("all")]
        public async Task LeaderboardCommand()
        {
            DirectoryInfo di = new DirectoryInfo("leaderboard");
            Dictionary<long, int> temp= new Dictionary<long, int>();
            List<Tuple<string,int>> d =  new List<Tuple<string, int>>();
            foreach(FileInfo file in di.GetFiles())
            {
                temp.Add(long.Parse(file.Name.Replace(".37","")),int.Parse(File.ReadAllText($"leaderboard/{file.Name}"))); 
            }
            foreach(KeyValuePair<long,int> kvp in temp)
            {
                string username = "Unknown User";
                var client = (DiscordSocketClient)Context.Client;
                var guildList = client.Guilds;
                foreach (SocketGuild guild in guildList)
                {
                    foreach (SocketUser user in guild.Users)
                    {
                        if((long)user.Id == kvp.Key)
                        {
                            username = user.Username;
                            goto stop;
                        }
                    }
                }
            stop:;
                Tuple<string, int> entry = new Tuple<string, int>(username,kvp.Value);
                d.Add(entry);
            }
            var leaderboard = from entry in d orderby entry.Item2 descending select entry;
            EmbedBuilder builder = new EmbedBuilder();
            string lb = "";
            int b = 1;
            builder.WithAuthor("37Gang-Leaderboard", "https://cdn.discordapp.com/app-icons/737060692527415466/c64109fbdff1a1f6dfd7515eaec5198d.png?size=512", "https://bit.ly/37status");
            builder.WithFooter("Accuracy of these values can not be guaranteed", "https://cdn.discordapp.com/emojis/734132648800419880.png");
            foreach (Tuple<string, int> kvp in leaderboard)
            {
                string count;
                if (kvp.Item2 == 1)
                    count = "37";
                else
                    count = "37s";
                lb = lb + "\n" + b + $". {kvp.Item1} ({kvp.Item2} {count})";
                b++;
            }
            Colorpicker picker = new Colorpicker();
            builder.WithColor((uint)picker.Pick());
            builder.WithDescription(lb);
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}