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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration.Json;
using botof37s;
using botof37s.services;
using colorpicker;

namespace botof37s.Modules
{


    public class Memecommand : ModuleBase
    {
        public IConfiguration _config { get; set; }
        public DiscordSocketClient _client { get; set; }
        public CommandHandler handler { get; set; }

        [Command("meme")]
        [Name("🤪 meme <sound> <username>")]
        [Summary("Plays a selected sound the next time the user joins a voice channel")]
        [Remarks("authorized")]
        public async Task MemeCommandAsync(string mp3 = null, [Remainder]string user  = null)
        {

            Authorisationcheck authorisationcheck = new Authorisationcheck();
            if (!authorisationcheck.Check(Context.User.Id, _config))
                return;

            if(mp3 == null||user == null)
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> You need to provide a sound as well as a username");
                return;
            }
            if (!File.Exists($"audio/{mp3}.wav"))
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> I'm sorry, but the audio file you requested doesnt exist (yet)");
                return;
            }
            
            string id = null;
            string username = "";
            var guilds = _client.Guilds;
            foreach (SocketGuild guild in guilds)
            {
                foreach (SocketUser user1 in guild.Users)
                {
                    if (user1.Username.ToLower() == user.ToLower())
                    {
                        id = user1.Id.ToString();
                        username = user1.Username;
                    }
                }
            }
            if(id == null)
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> I'm having trouble finding that user, please try again");
                return;
            }
            
            File.WriteAllText($"prank/{id}.37", mp3);
            await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Successfully queued the user for a meme!");
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithFooter("No copyright infringement intended Kappa", "https://cdn.discordapp.com/emojis/734132648800419880.png");
            var tfile = TagLib.File.Create($"audio/{mp3}.wav");
            string song = tfile.Tag.Title;
            string artist = tfile.Tag.Performers.FirstOrDefault();
            string thumb = tfile.Tag.Comment;
            builder.WithThumbnailUrl(thumb);
            builder.WithTitle($"Queued meme for user **{username}**");
            builder.WithDescription($"**{song}** by **{artist}**");
            Colorpicker picker = new Colorpicker();
            builder.WithColor(picker.Pick());
            await Context.Channel.SendMessageAsync("", false, builder.Build());

        }
    }
}