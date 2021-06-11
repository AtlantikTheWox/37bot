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
using System.Net;
using System.Drawing;
using System.Text;

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
        public LogService lservice;
        public Dictionary<ulong, Tuple<IAudioClient, Process>> _connections;

        public CommandHandler(IServiceProvider services)
        {
            
            _config = services.GetRequiredService<IConfiguration>();
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            twitchclient = services.GetRequiredService<TwitchClient>();
            _connections = services.GetRequiredService<Dictionary<ulong, Tuple<IAudioClient, Process>>>();
            lservice = services.GetRequiredService<LogService>();
            _services = services;

            
            _commands.CommandExecuted += CommandExecutedAsync;
            _client.Ready += ReadyAsync;
            _client.UserVoiceStateUpdated += OnVoiceStateUpdated;
            _client.Connected += ConnectedAsync;

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
                if (message.Content.ToLower().Replace(" ","").Contains("furry")&& new Random().Next(10)==6)
                {
                    var missage = await message.Channel.SendMessageAsync("!quote 1");
                    delmessig = missage.Id.ToString();
                }
                else if (message.Content.Contains(" 37"))
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
                lservice.LogAsync($"Command [{command.Value.Name}] executed",LogLevel.Default,context);
                return;
            }


            // failure scenario, let's let the user know
            await context.Channel.SendMessageAsync($"<@{context.User.Id}> something went wrong -> [{result}]!");
            await lservice.LogAsync($"Command execution failed with exception: {result}", LogLevel.Error,context);

        }
        private Task ReadyAsync()
        {
            if(twitchclient.IsConnected)
                twitchclient.Reconnect();
            Console.WriteLine($"Connected as -> [{_client.CurrentUser}]");
            if(File.Exists("db/customtime.37"))
            {
                TimeSpan ts = DateTime.UtcNow - Convert.ToDateTime(File.ReadAllText("db/customtime.37"));
                if (ts.TotalMinutes > 90)
                {
                    custom = false;
                    File.Delete("db/customtime.37");
                }
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
                    await state1.VoiceChannel.DisconnectAsync();
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
                        ConnectToVoice(state2.VoiceChannel, File.ReadAllText($"prank/{user.Id}.37"),user.Id.ToString());
                    }
                }
            }
        }
        private Task ConnectedAsync()
        {
            if (File.Exists($"wheelspoof/tokens/{_config["AdminUserID"]}.37")) Autoroll(File.ReadAllText($"wheelspoof/tokens/{_config["AdminUserID"]}.37"));
            Puro();
            return Task.CompletedTask;
        }
        public void customTrue()
        {
            File.WriteAllText("db/customtime.37", DateTime.UtcNow.ToString());
            custom = true;
        }
        public void customFalse()
        {
            custom = false;
            File.Delete("db/customtime.37");
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
        private async Task Puro()
        {
            try
            {
                DateTime lastpuro = new DateTime();
                if (File.Exists("db/lastpuro.37"))
                {
                    lastpuro = DateTime.Parse(File.ReadAllText("db/lastpuro.37"));
                }
                TimeSpan ts = DateTime.UtcNow - lastpuro;
                if (ts.TotalHours > 6)
                {
                    WebClient client = new WebClient();
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    client.Encoding = Encoding.GetEncoding("gbk");
                    string purotext = await client.DownloadStringTaskAsync("https://item.taobao.com/item.htm?spm=a2oq0.12575281.0.0.16c11debsIvMJ8&ft=t&id=609352329954");
                    if (!purotext.Contains("此宝贝已下架"))
                    {
                        var user = _client.GetUser(ulong.Parse(_config["AdminUserID"]));
                        var simmo = _client.GetUser(262245466895417344);
                        await user.SendMessageAsync("P U R O  <https://item.taobao.com/item.htm?spm=a2oq0.12575281.0.0.16c11debsIvMJ8&ft=t&id=609352329954>");
                        await simmo.SendMessageAsync("goo boy");
                        File.WriteAllText("db/lastpuro.37", DateTime.MaxValue.ToString());
                    }
                    _ = client;
                    File.WriteAllText("db/lastpuro.37", DateTime.UtcNow.ToString());
                }
            }catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private async Task Autoroll(string token)
        {
            if (File.Exists($"wheelspoof/{token}")) return;
            if (File.Exists($"wheelspoof/autoroll/{_config["AdminUserID"]}.37"))
            {
                DateTime lastwin = DateTime.Parse(File.ReadAllText($"wheelspoof/autoroll/{_config["AdminUserID"]}.37"));
                TimeSpan ts = DateTime.UtcNow - lastwin;
                if(ts.TotalHours < 22)
                {
                    return;
                } 
            }
            await File.WriteAllTextAsync($"wheelspoof/{token}", "uwu");
            var user = _client.GetUser(ulong.Parse(_config["AdminUserID"]));
            try
            {
                HttpClient client = new HttpClient();
                Dictionary<int, int> distrib = new Dictionary<int, int>
                {
                { 0, 0 },
                { 1, 0 },
                { 2, 0 },
                { 3, 0 },
                { 4, 0 },
                { 5, 0 },
                { 6, 0 },
                { 7, 0 },
                { 8, 0 },
                { 9, 0 },
                { 10, 0 },
                { 11, 0 },

                };
                List<string> agents = new List<string>()
                {
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0",
                "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/5352 (KHTML, like Gecko) Chrome/37.0.825.0 Mobile Safari/5352",
                "Opera/9.79 (X11; Linux i686; en-US) Presto/2.10.342 Version/10.00",
                "Mozilla/5.0 (iPhone; CPU iPhone OS 7_0_2 like Mac OS X; en-US) AppleWebKit/532.43.6 (KHTML, like Gecko) Version/3.0.5 Mobile/8B115 Safari/6532.43.6",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.90 Safari/537.36"
                };
                client.DefaultRequestHeaders.Add("Accept", "application / json, text / plain, */*");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                client.DefaultRequestHeaders.Add("Host", "gamblingbot.app");
                client.DefaultRequestHeaders.Add("Pragma", "no-cache");
                client.DefaultRequestHeaders.Add("Referer", "https://gamblingbot.app/games/fortune-wheel");
                client.DefaultRequestHeaders.Add("User-Agent", agents[new Random().Next(agents.Count())]);
                DateTime start = DateTime.UtcNow;
                bool mil = false;
                bool startconfirmation = false;
                HttpResponseMessage cooldownresponse = await client.GetAsync("https://gamblingbot.app/api/games/fortune-wheel-free-cooldown");
                if (await cooldownresponse.Content.ReadAsStringAsync() == "invalid user")
                {
                    await user.SendMessageAsync($"Your token expired again you dingus!");
                    Console.WriteLine("Invalid free cooldown");
                    if (File.Exists($"wheelspoof/{token}")) File.Delete($"wheelspoof/{token}");
                    if (File.Exists($"wheelspoof/tokens/{user.Id}.37")) File.Delete($"wheelspoof/tokens/{user.Id}.37");
                    return;
                }
                else
                {
                    DateTimeOffset unixstart = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(await cooldownresponse.Content.ReadAsStringAsync()));
                    unixstart = unixstart.AddHours(22);
                    if (DateTime.UtcNow.CompareTo(unixstart.UtcDateTime) < 0)
                    {   
                        if (File.Exists($"wheelspoof/{token}")) File.Delete($"wheelspoof/{token}");
                        return;
                    }
                }
                int failedrequests = 0;
                while (!mil)
                {
                    HttpResponseMessage response = await client.GetAsync($"https://gamblingbot.app/api/games/fortune-wheel-start");
                    string responsestring = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        failedrequests = 0;
                        if (responsestring == "invalid user")
                        {
                            await user.SendMessageAsync($"<@{user.Id}> The allmighty bot didnt like that one. This may be because your token has expired or because the gambling bot is having technical difficulties.");
                            if (File.Exists($"wheelspoof/{token}")) File.Delete($"wheelspoof/{token}");
                            return;
                        }
                        if (!startconfirmation)
                        {
                            string confirmationresponse = $"<@{user.Id}> I have started up the automatic spin feature for you.";
                            startconfirmation = true;
                            await user.SendMessageAsync(confirmationresponse);
                        }
                        try
                        {

                            int key = int.Parse(responsestring.Replace("{\"win\":", "").Replace("}", ""));
                            distrib[key]++;
                        }
                        catch (FormatException)
                        {
                            int counter = 0;
                            foreach (KeyValuePair<int, int> kvp in distrib)
                            {
                                counter += kvp.Value;
                            }
                            TimeSpan temp = DateTime.UtcNow - start;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"{DateTime.UtcNow}: Error in autoroll wheel response for {user.Username}#{user.Discriminator}: \"{responsestring}\"");
                            Console.ResetColor();
                            lservice.LogAsync($"Invalid Response: \"{responsestring}\"", LogLevel.Severe);
                            await user.SendMessageAsync($"<@{user.Id}> Autoroll: Oops, i didnt expect the following response after {counter} rolls and {temp.Hours}h {temp.Minutes}m {temp.Seconds}s: \"{responsestring}\" Please contact my owner with this information or try again");
                            File.Delete($"wheelspoof/{token}");
                            return;
                        }
                        if (responsestring == "{\"win\":0}")
                        {
                            mil = true;
                            await Task.Delay(7000);
                        }
                    }
                    else
                    {
                        if (failedrequests < 10)
                        {
                            failedrequests++;
                        }
                        else
                        {
                            int counter = 0;
                            foreach (KeyValuePair<int, int> kvp in distrib)
                            {
                                counter += kvp.Value;
                            }
                            TimeSpan temp = DateTime.UtcNow - start;
                            await user.SendMessageAsync($"<@{user.Id}> Autowheel: I'm sorry, but the last 10 requests failed after {counter} sucessfull rolls and a time of {temp.Hours} hours and {temp.Minutes} minutes. The last request failed with the status code \"{response.StatusCode}\" and the response \"{responsestring}\". Please contact my owner with this info or try again later. ");
                            File.Delete($"wheelspoof/{token}");
                            return;
                        }
                    }
                    await Task.Delay(5000 + new Random().Next(-1000, 1000));
                }
                HttpResponseMessage winresponse = await client.GetAsync($"https://gamblingbot.app/api/games/fortune-wheel-ok");
                TimeSpan ts = DateTime.UtcNow - start;
                WebClient web = new WebClient();
                Stream stream = web.OpenRead($"https://quickchart.io/chart?bkg=white&c={{type:'bar',data:{{labels:['10,000,000','9,000','45,000','11,500','450,000','15,000','60,000','20,000','250,000','30,000','120,000','6,000'],datasets:[{{ label:'Times rolled',data:[{distrib[0]},{distrib[1]},{distrib[2]},{distrib[3]},1{distrib[4]},{distrib[5]},{distrib[6]},{distrib[7]},{distrib[8]},{distrib[9]},{distrib[10]},{distrib[11]}]}}]}}}}");
                Bitmap bitmap = new Bitmap(stream);
                bitmap.Save($"wheelspoof/barcharts/{user.Id}.png", System.Drawing.Imaging.ImageFormat.Png);
                File.Delete($"wheelspoof/{token}");
                int finalcounter = 0;
                foreach (KeyValuePair<int, int> kvp in distrib)
                {
                    finalcounter += kvp.Value;
                }
                File.WriteAllText($"wheelspoof/autoroll/{user.Id}.37", DateTime.UtcNow.ToString());
                await user.SendMessageAsync($"<@{user.Id}> Autoroll: Success! After {ts.Hours} hours, {ts.Minutes} minutes, {ts.Seconds} seconds and {finalcounter} rolls I have managed to make a winning roll! Raw response: \"{await winresponse.Content.ReadAsStringAsync()}\"  Distribution chart: ");
                await user.SendFileAsync($"wheelspoof/barcharts/{user.Id}.png");
                File.Delete($"wheelspoof/barcharts/{user.Id}.png");
            }
            catch (Exception e)
            {
                if (File.Exists($"wheelspoof/{token}")) File.Delete($"wheelspoof/{token}");
                await user.SendMessageAsync($"Autoroll fail: {e}");
            }
        }
    }
}