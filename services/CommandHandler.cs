using System;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

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

        public CommandHandler(IServiceProvider services)
        {
            // juice up the fields with these services
            // since we passed the services in, we can use GetRequiredService to pass them into the fields set earlier
            _config = services.GetRequiredService<IConfiguration>();
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            // take action when we execute a command
            _commands.CommandExecuted += CommandExecutedAsync;
            _client.Ready += ReadyAsync;
            // take action when we receive a message (so we can process it, and see if it is a valid command)
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            // register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        // this class is where the magic starts, and takes actions upon receiving messages
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // ensures we don't process system/other bot messages
            if (!(rawMessage is SocketUserMessage message))
            {
                return;
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
            if (message.Content.StartsWith($"AUTH {_client.CurrentUser.Id}") && message.Author.IsBot)
            {
                
                if (delmessig == null)
                {
                    Console.WriteLine("messig null");
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

            // sets the argument position away from the prefix we set
            var argPos = 0;
            if (rawMessage.Content == _config["Prefix"].Replace(" ", ""))
            {
                goto dm;
            }
            if (rawMessage.Channel.GetType().ToString() == "Discord.WebSocket.SocketDMChannel"&& !message.Content.StartsWith(_config["Prefix"]) && !message.Content.StartsWith(_client.CurrentUser.ToString()))
            {
                goto dm;
            }
            // get prefix from the configuration file
            string prefix = _config["Prefix"];

            // determine if the message has a valid prefix, and adjust argPos based on prefix
            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.HasStringPrefix(prefix, ref argPos)))
            {
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
            Console.WriteLine($"Connected as -> [{_client.CurrentUser}]");
            if(File.Exists("db/customtime.37"))
            {
                
                TimeSpan ts = DateTime.UtcNow - Convert.ToDateTime(File.ReadAllText("db/customtime.37"));
                Console.WriteLine(ts);
                if (ts.TotalMinutes > 90)
                    custom = false;
            }
            if (!custom)
            {
                Activitypicker picker = new Activitypicker();
                picker.Pick(_client);
            }
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
        }
    }
}