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

namespace botof37s.Modules
{



    public class VCcommand : ModuleBase
    {
        public TwitchClient twitchclient { get; set; }
        public IConfiguration _config { get; set; }
        public CommandHandler handler { get; set; }
        public DiscordSocketClient _client { get; set; }
        public Dictionary<ulong, IAudioClient> _connections { get; set; }
        [Command("vc", RunMode = RunMode.Async)]
        [Summary("Manages the voice chat feature of the bot")]
        [Name("🗣 vc")]
        [Remarks("all")]
        public async Task VCCommand(string sound = null)
        {
            if((Context.User as IVoiceState).VoiceChannel == null || Context.Guild.Id != (Context.User as IVoiceState).VoiceChannel.GuildId)
            {
                await Context.Channel.SendMessageAsync("You need to be in a vc on this server to use the vc command 🤪");
                return;
            }
            var guild = (Context.User as IVoiceState).VoiceChannel.Guild;
            IAudioClient connection = _connections.GetValueOrDefault(guild.Id);
            
            if (connection != null && connection.ConnectionState == Discord.ConnectionState.Connected)
            {
                if(sound == "dc")
                {
                    if((Context.User as IVoiceState).VoiceChannel == null)
                    {
                        await Context.Channel.SendMessageAsync("You need to be in the same vc as the bot to use this command!");
                        return;
                    }
                    else
                    {
                        await (Context.User as IVoiceState).VoiceChannel.DisconnectAsync();
                        await Context.Channel.SendMessageAsync("Disconnected!");
                        return;
                    }
                }
                await Context.Channel.SendMessageAsync("Sorry, but i cant be in 2 channels at once");
                return;
            }
            if (connection == null || connection.ConnectionState != Discord.ConnectionState.Connected)
            {

                if (sound == null)
                    sound = "hawwy";
                if (!File.Exists($"audio/{sound.ToLower()}.mp3"))
                {
                    await Context.Channel.SendMessageAsync("I'm sorry, but i dont know that sound (yet)");
                    return;
                }
                try
                {
                    Console.WriteLine($"Connecting to channel {((Context.User as IVoiceState)).VoiceChannel.Id}");
                    var audioclient = await (Context.User as IVoiceState).VoiceChannel.ConnectAsync();
                    Console.WriteLine($"Connected to channel {((SocketVoiceChannel)((Context.User as IVoiceState).VoiceChannel)).Id}");
                    _connections[((SocketVoiceChannel)((Context.User as IVoiceState).VoiceChannel)).Guild.Id] = audioclient;
                    connection = audioclient;
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    // Oh no, error
                    Console.WriteLine(ex.Message);
                    Console.WriteLine($"- {ex.StackTrace}");
                }
                await Context.Channel.SendMessageAsync("Connected!");
                
                try
                {
                    await connection.SetSpeakingAsync(true); // send a speaking indicator

                    var psi = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $@"-i ""audio/{sound}.mp3"" -ac 2 -f s16le -ar 48000 pipe:1",
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    };
                    var ffmpeg = Process.Start(psi);

                    var output = ffmpeg.StandardOutput.BaseStream;
                    var discord = connection.CreatePCMStream(AudioApplication.Voice);
                    await output.CopyToAsync(discord);
                    await discord.FlushAsync();

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
                }catch
                { }
            }
        }
    }
}