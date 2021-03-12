using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Audio;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Data;
using Discord.Rest;
using Microsoft.Extensions.Configuration.Json;
using botof37s;
using botof37s.services;
using colorpicker;
using botof37s.Modules;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Enums;
using System.Diagnostics;
using TagLib;
using File = System.IO.File;

namespace botof37s.Modules
{



    public class VCcommand : ModuleBase
    {
        public TwitchClient twitchclient { get; set; }
        public IConfiguration _config { get; set; }
        public CommandHandler handler { get; set; }
        public DiscordSocketClient _client { get; set; }
        public Dictionary<ulong, Tuple<IAudioClient, Process>> _connections { get; set; }
        [Command("vc", RunMode = RunMode.Async)]
        [Summary("Manages the voice chat feature of the bot")]
        [Name("🗣 vc <sound name|list|random>")]
        [Remarks("all")]
        public async Task VCCommand(string sound = null)
        {
            if(sound == "list")
            {
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithAuthor("37 vc sounds", "https://cdn.discordapp.com/app-icons/737060692527415466/c64109fbdff1a1f6dfd7515eaec5198d.png?size=512", "https://bit.ly/37status");
                builder.WithFooter("No copyright infringement intended Kappa", "https://cdn.discordapp.com/emojis/734132648800419880.png");
                DirectoryInfo di = new DirectoryInfo("audio");
                foreach(FileInfo file in di.GetFiles())
                {
                    if (file.Name != "hawwy.wav")
                    {

                        var tfile = TagLib.File.Create($"audio/{file.Name}");
                        string name = file.Name.Replace(".wav", "");
                        string song = tfile.Tag.Title;
                        string artist = tfile.Tag.Performers.FirstOrDefault();
                        builder.AddField(name, $"**{song}** by **{artist}**");
                        
                    }

                }
                Colorpicker picker = new Colorpicker();
                builder.WithColor((uint)picker.Pick());
                await Context.Channel.SendMessageAsync("", false, builder.Build());
                return;
                
            }
            if ((Context.User as IVoiceState).VoiceChannel == null || Context.Guild.Id != (Context.User as IVoiceState).VoiceChannel.GuildId)
            {
                await Context.Channel.SendMessageAsync("You need to be in a vc on this server to use the vc command 🤪");
                return;
            }
            var guild = (Context.User as IVoiceState).VoiceChannel.Guild;
            var def = new Tuple<IAudioClient, Process>(null, null);
            IAudioClient connection;
            try
            {
                connection = _connections.GetValueOrDefault(guild.Id).Item1;
            }
            catch(System.NullReferenceException)
            {
                connection = null;
            }
            if (sound == "dc")
            {
                if(connection != null && connection.ConnectionState == Discord.ConnectionState.Connected)
                {
                    if ((Context.User as IVoiceState).VoiceChannel == null)
                    {
                        await Context.Channel.SendMessageAsync("You need to be in the same vc as the bot to use this command!");
                        return;
                    }
                    else
                    {
                        await (Context.User as IVoiceState).VoiceChannel.DisconnectAsync();
                        if (_connections.GetValueOrDefault(guild.Id).Item2 != null)
                        {
                            _connections.GetValueOrDefault(guild.Id).Item2.Kill();
                            Tuple<IAudioClient, Process> te = new Tuple<IAudioClient, Process>(connection, null);
                            _connections[((SocketVoiceChannel)((Context.User as IVoiceState).VoiceChannel)).Guild.Id] = te;
                        }
                        await Context.Channel.SendMessageAsync("Disconnected!");
                        return;
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync("You cant disconnect the bot if its not connected");
                    return;
                }
            }
            if (connection != null && connection.ConnectionState == Discord.ConnectionState.Connected)
            {
                await Context.Channel.SendMessageAsync("Sorry, but i cant be in 2 channels at once");
                return;
            }
            if (connection == null || connection.ConnectionState != Discord.ConnectionState.Connected)
            {

                if (sound == null)
                    sound = "hawwy";
                if (sound == "random")
                {
                    DirectoryInfo di = new DirectoryInfo("audio");
                    var r = new Random();
                    int rand = r.Next(di.GetFiles().Count());
                    sound = di.GetFiles()[rand].Name.Replace(".wav", "");
                }
                if (!File.Exists($"audio/{sound.ToLower()}.wav"))
                {
                    await Context.Channel.SendMessageAsync("I'm sorry, but i dont know that sound (yet). Check \"/37 vc list\" for a list of available sounds\"");
                    return;
                }
                try
                {
                    Console.WriteLine($"Connecting to channel {((Context.User as IVoiceState)).VoiceChannel.Id}");
                    var audioclient = await (Context.User as IVoiceState).VoiceChannel.ConnectAsync();
                    Console.WriteLine($"Connected to channel {((SocketVoiceChannel)((Context.User as IVoiceState).VoiceChannel)).Id}");
                    Tuple<IAudioClient, Process> t = new Tuple<IAudioClient, Process>(audioclient, null);
                    _connections[((SocketVoiceChannel)((Context.User as IVoiceState).VoiceChannel)).Guild.Id] = t;
                    connection = audioclient;
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    // Oh no, error
                    Console.WriteLine(ex.Message);
                    Console.WriteLine($"- {ex.StackTrace}");
                }
                EmbedBuilder builder = new EmbedBuilder();
                
                builder.WithFooter("No copyright infringement intended Kappa", "https://cdn.discordapp.com/emojis/734132648800419880.png");
                var tfile = TagLib.File.Create($"audio/{sound}.wav");
                string song = tfile.Tag.Title;
                string artist = tfile.Tag.Performers.FirstOrDefault();
                string thumb = tfile.Tag.Comment;
                builder.WithThumbnailUrl(thumb);
                builder.WithTitle($"Now playing");
                builder.WithDescription($"**{song}** by **{artist}**");
                Colorpicker picker = new Colorpicker();
                builder.WithColor(picker.Pick());
                await Context.Channel.SendMessageAsync("", false, builder.Build());
                

                try
                {
                    await connection.SetSpeakingAsync(true); // send a speaking indicator

                    var psi = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $@"-re -i ""audio/{sound}.wav"" -ac 2 -f s16le -ar 48000 pipe:1",
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    };
                    var ffmpeg = Process.Start(psi);
                    Tuple<IAudioClient, Process> t = new Tuple<IAudioClient, Process>(connection, ffmpeg);
                    _connections[((SocketVoiceChannel)((Context.User as IVoiceState).VoiceChannel)).Guild.Id] = t;
                    var output = ffmpeg.StandardOutput.BaseStream;
                    var discord = connection.CreatePCMStream(AudioApplication.Voice);
                    await output.CopyToAsync(discord);
                    await discord.FlushAsync();
                    Tuple<IAudioClient, Process> te = new Tuple<IAudioClient, Process>(connection, null);
                    _connections[((SocketVoiceChannel)((Context.User as IVoiceState).VoiceChannel)).Guild.Id] = te;

                    await connection.SetSpeakingAsync(false); // we're not speaking anymore
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine($"- {ex.StackTrace}");
                }
                try
                {
                    await ((SocketVoiceChannel)(Context.User as IVoiceState).VoiceChannel).DisconnectAsync();
                }
                catch(System.NullReferenceException)
                {

                }
            }
        }
    }
}