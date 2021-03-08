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

namespace botof37s.Modules
{


    public class Memecommand : ModuleBase
    {
        public IConfiguration _config { get; set; }
        public DiscordSocketClient _client { get; set; }
        public CommandHandler handler { get; set; }

        [Command("meme")]
        public async Task MemeCommandAsync(string mp3 = null, [Remainder]string user  = null)
        {

            Authorisationcheck authorisationcheck = new Authorisationcheck();
            if (!authorisationcheck.Check(Context.User.Id, _config))
                return;

            if(mp3 == null||user == null)
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> You need to provide an mp3 as well as a username");
                return;
            }
            if (!File.Exists($"audio/{mp3}.mp3"))
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> I'm sorry, but the audio file you requested doesnt exist (yet)");
                return;
            }
            SocketGuild guild = (SocketGuild)Context.Guild;
            string id = null;
            foreach(SocketUser user1 in guild.Users)
            {
                if(user1.Username.ToLower() == user.ToLower())
                {
                    id = user1.Id.ToString();
                }
            }
            if(id == null)
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> I'm having trouble finding that user, please try again");
                return;
            }
            if (File.Exists($"prank/{id}.37"))
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> I'm sorry, but that user is already queued up for a meme. Please try again later.");
                return;
            }
            File.WriteAllText($"prank/{id}.37", mp3);
            await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Successfully queued the user for a meme!");

        }
    }
}