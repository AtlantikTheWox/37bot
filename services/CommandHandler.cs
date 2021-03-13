using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Rest;
using Discord.Audio;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
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
using System.Linq;
using botof37s.Modules;
using System.Diagnostics;
using System.Net.Http;

namespace botof37s.services
{
    public class CommandHandler
    {
        // setup fields to be set later in the constructor
        public readonly IConfiguration _config;
        private readonly CommandService _commands;
        public DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        string delmessig = null;
        bool custom = false;
        public TwitchClient twitchclient;
        public Dictionary<ulong, Tuple<IAudioClient, Process>> _connections;

        public CommandHandler(IServiceProvider services)
        {
            
            _config = services.GetRequiredService<IConfiguration>();
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            twitchclient = services.GetRequiredService<TwitchClient>();
            _connections = services.GetRequiredService<Dictionary<ulong, Tuple<IAudioClient, Process>>>();
            _services = services;

            
            _commands.CommandExecuted += CommandExecutedAsync;
            _client.Ready += ReadyAsync;
            _client.UserVoiceStateUpdated += OnVoiceStateUpdated;

            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            // register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // ensures we don't process system/other bot messages
            if (!(rawMessage is SocketUserMessage message))
            {
                return;
            }
            if (message.Content.StartsWith($"AUTH {_client.CurrentUser.Id}") && message.Author.IsBot)
            {

                if (delmessig == null)
                {
                    return;
                }
                else
                {
                    try
                    {
                        string[] subs = message.Content.Split(" ");
                        double verify = Convert.ToDouble(subs[2]);
                        double key = Convert.ToDouble(Decimal.Round(Convert.ToDecimal(Double.Parse(delmessig) * verify), 14));
                        await message.Channel.SendMessageAsync($"AUTHKEY {key}");
                        delmessig = null;
                    }
                    catch (Exception e)
                    {
                        await message.Channel.SendMessageAsync(e.ToString());
                    }
                }
            }

            if (message.Source != MessageSource.User)
            {
                return;
            }

            //REMOVE
            if(message.Author.Id != ulong.Parse("329650083819814913"))
            {
                return;
            }
            //REMOVE
            if (message.Content.Replace(" ", "").ToLower().Contains("<:37c:712802406173245460><:37b:712802398854316052>"))
            {
                var guildList = _client.Guilds;
                foreach (SocketGuild guild in guildList)
                {
                    if (guild.Id.ToString() == "608332317290921996")
                    {
                        await message.DeleteAsync();
                        foreach (IMessageChannel channel in guild.TextChannels)
                        {
                            if (channel.Id.ToString() == "646779838044176414")
                            {

                                await channel.SendMessageAsync($"EMOTE MISUSE DETECTED: User:\"{message.Author.Username}\" Message:\"{message.Content}\" They have been put on the naughty step.");
                                var sentmessage = await channel.SendMessageAsync($"/sb mute {message.Author.Id}");
                                delmessig = sentmessage.Id.ToString();
                                return;
                            }
                        }
                    }
                }

            }
            

            // sets the argument position away from the prefix we set
            var argPos = 0;
            if (rawMessage.Content == _config["Prefix"].Replace(" ", ""))
            {
                goto dm;
            }
            if (rawMessage.Channel.GetType().ToString() == "Discord.WebSocket.SocketDMChannel" && !message.Content.StartsWith(_config["Prefix"]) && !message.Content.StartsWith(_client.CurrentUser.ToString()))
            {
                goto dm;
            }
            // get prefix from the configuration file
            string prefix = _config["Prefix"];

            // determine if the message has a valid prefix, and adjust argPos based on prefix
            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.HasStringPrefix(prefix, ref argPos)))
            {
                Random r = new Random();
                if (message.Content.ToLower().Contains("furry"))
                {
                    var missage = await message.Channel.SendMessageAsync("!quote 1");
                    delmessig = missage.Id.ToString();
                }
                else if(message.Content.Equals("!quote 1") && r.Next(11) == 3)
                {
                    //April Fools :LUL:
                    if (DateTime.UtcNow.Month == 4 && DateTime.UtcNow.Day == 1)
                    {
                        await (Task.Delay(200));
                        IMessageChannel contextchannel = (IMessageChannel)message.Channel;
                        var retrieval = contextchannel.GetMessagesAsync(message, Direction.After, 1).Flatten();
                        var simbotmessig = await retrieval.LastOrDefaultAsync();
                        try
                        {
                            simbotmessig.DeleteAsync();
                        }
                        catch(Exception)
                        {
                            
                        }
                    }
                    EmbedBuilder builder = new EmbedBuilder();
                    builder.WithAuthor("Quote #1", "https://images-ext-1.discordapp.net/external/HugnJG4NH2p5vdh4FUhaT9i7HF2T_1VhvqbttpWmst8/https/cdn.discordapp.com/icons/608332317290921996/9a2aee39533d63eea03a841621b8e491.jpg");
                    builder.WithColor(3447003);
                    builder.WithDescription("I am a fucking furry! I fuck those guys!");
                    builder.WithFooter("Saved at some point in Ye Olde Times by Simmotipo#6877", "https://images-ext-1.discordapp.net/external/HugnJG4NH2p5vdh4FUhaT9i7HF2T_1VhvqbttpWmst8/https/cdn.discordapp.com/icons/608332317290921996/9a2aee39533d63eea03a841621b8e491.jpg");
                    await message.Channel.SendMessageAsync(null,false,builder.Build()); 
                }
                else if (message.Content.Contains("37 "))
                {
                    try
                    {
                        var m = (RestUserMessage)await message.Channel.GetMessageAsync(message.Id);
                        var guildList = _client.Guilds;
                        foreach (SocketGuild guild in guildList)
                        {
                            foreach (IEmote emote in guild.Emotes)
                            {
                                if (emote.Name.ToString() == "37a")
                                {
                                    await m.AddReactionAsync(emote);
                                }
                            }
                        }
                        foreach (SocketGuild guild in guildList)
                        {
                            foreach (IEmote emote in guild.Emotes)
                            {
                                if (emote.Name.ToString() == "37b")
                                {
                                    await m.AddReactionAsync(emote);
                                }
                            }
                        }
                        foreach (SocketGuild guild in guildList)
                        {
                            foreach (IEmote emote in guild.Emotes)
                            {
                                if (emote.Name.ToString() == "37c")
                                {
                                    await m.AddReactionAsync(emote);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    return;
                }
                return;
            }

            dm:;
            var context = new SocketCommandContext(_client, message);

            // execute command if one is found that matches
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // if a command isn't found, log that info to console and exit this method
            if (!command.IsSpecified)
            {
                await context.Channel.SendMessageAsync($"<@{context.User.Id}> Dafuq you want?");
                return;
            }


            // log success to the console and exit this method
            if (result.IsSuccess)
            {
                System.Console.WriteLine($"Command [{command.Value.Name}] executed for -> [{context.User.Username}]");
                return;
            }


            // failure scenario, let's let the user know
            await context.Channel.SendMessageAsync($"<@{context.User.Id}> something went wrong -> [{result}]!");
        }
        private Task ReadyAsync()
        {
            twitchclient.Reconnect();
            Console.WriteLine($"Connected as -> [{_client.CurrentUser}]");
            if(File.Exists("db/customtime.37"))
            {
                TimeSpan ts = DateTime.UtcNow - Convert.ToDateTime(File.ReadAllText("db/customtime.37"));
                if (ts.TotalMinutes > 90)
                    custom = false;
                File.Delete("db/customtime.37");
            }
            if (!custom)
            {
                Activitypicker picker = new Activitypicker();
                picker.Pick(_client);
            }
            return Task.CompletedTask;
        }
        private async Task OnVoiceStateUpdated(SocketUser user, SocketVoiceState state1, SocketVoiceState state2)
        {
            // Check if this was a non-bot user joining a voice channel
            if (user.IsBot)
                return;

            var guild = state2.VoiceChannel?.Guild ?? state1.VoiceChannel?.Guild;
            if (guild == null)
                return;
            IAudioClient connection;
            try
            {
                connection = _connections.GetValueOrDefault(guild.Id).Item1;
            }
            catch(System.NullReferenceException)
            {
                connection = null;
            }
            if (state2.VoiceChannel == null && state1.VoiceChannel != null && connection != null)
            {
                // Disconnected
                if (!state1.VoiceChannel.Users.Any(u => !u.IsBot))
                {
                    await state1.VoiceChannel.DisconnectAsync();
                    if (_connections.GetValueOrDefault(guild.Id).Item2 != null)
                    {
                        _connections.GetValueOrDefault(guild.Id).Item2.Kill();
                        Tuple<IAudioClient, Process> te = new Tuple<IAudioClient, Process>(connection, null);
                        _connections[guild.Id] = te;
                    }
                }
                return;
            }

            if (connection == null || connection.ConnectionState != ConnectionState.Connected)
            {
                if (File.Exists($"prank/{user.Id}.37"))
                {
                    if (File.Exists($"audio/{File.ReadAllText($"prank/{user.Id}.37")}.wav"))
                    {
                        await Task.Delay(1000);
                        ConnectToVoice(state2.VoiceChannel, File.ReadAllText($"prank/{user.Id}.37"),user.Id.ToString()).Start();
                    }
                }
            }
        }
        public void customTrue()
        {
            File.WriteAllText("db/customtime.37", DateTime.UtcNow.ToString());
            custom = true;
        }
        public void customFalse()
        {
            custom = false;
        }
        public async Task FileExpiryAsync(string key)
        {
            await Task.Delay(1000 * 60* 3);
            if (File.Exists($"{key}.37"))
            {
                File.Move($"{key}.37", $"{key}_expired.37");
            }
            await Task.Delay(1000 * 60 * 10);
            if (File.Exists($"{key}_expired.37"))
            {
                File.Delete($"{key}_expired.37");
            }
        }
        private async Task ConnectToVoice(SocketVoiceChannel voiceChannel, string sound, string id)
        {
            if (voiceChannel == null)
                return;

            try
            {
                Console.WriteLine($"Connecting to channel {voiceChannel.Id}");
                var connection = await voiceChannel.ConnectAsync();
                Console.WriteLine($"Connected to channel {voiceChannel.Id}");
                Tuple<IAudioClient, Process> t = new Tuple<IAudioClient, Process>(connection, null);
                _connections[voiceChannel.Guild.Id] = t;
                await Task.Delay(3000);
                await Say(connection, sound, voiceChannel);
                await Task.Delay(1000);
                await voiceChannel.DisconnectAsync();
                File.Delete($"prank/{id}.37");
            }
            catch (Exception ex)
            {
                // Oh no, error
                Console.WriteLine(ex.Message);
                Console.WriteLine($"- {ex.StackTrace}");
            }
        }
        private async Task Say(IAudioClient connection, string sound, SocketVoiceChannel channel)
        {
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

                var output = ffmpeg.StandardOutput.BaseStream;
                Tuple<IAudioClient, Process> t = new Tuple<IAudioClient, Process>(connection, ffmpeg);
                _connections[channel.Guild.Id] = t;
                var discord = connection.CreatePCMStream(AudioApplication.Voice);
                await output.CopyToAsync(discord);
                await discord.FlushAsync();
                Tuple<IAudioClient, Process> te = new Tuple<IAudioClient, Process>(connection, null);
                _connections[channel.Guild.Id] = te;
                await connection.SetSpeakingAsync(false); // we're not speaking anymore
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"- {ex.StackTrace}");
            }
        }
    }
}