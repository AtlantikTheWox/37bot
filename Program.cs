using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Rest;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using botof37s.services;
using botof37s.Modules;
using Discord.Commands;

namespace botof37s
{
    class Program
    {
        // setup our fields we assign later
        public readonly IConfiguration _config;
        public DiscordSocketClient _client;

        static void Main(string[] args)
        {
            if (!Directory.Exists("db")) Directory.CreateDirectory("db");
            if (!Directory.Exists("leaderboard")) Directory.CreateDirectory("leaderboard");
            if (!Directory.Exists("authorized")) Directory.CreateDirectory("authorized");
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            // create the configuration
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");

            // build the configuration and assign to _config          
            _config = _builder.Build();
        }

        public async Task MainAsync()
        {
            // call ConfigureServices to create the ServiceCollection/Provider for passing around the services
            using (var services = ConfigureServices())
            {
                // get the client and assign to client 
                // you get the services via GetRequiredService<T>
                var client = services.GetRequiredService<DiscordSocketClient>();
                _client = client;

                // setup logging and the ready event
                client.Log += LogAsync;
                
                services.GetRequiredService<CommandService>().Log += LogAsync;

                // this is where we get the Token value from the configuration file, and start the bot
                await client.LoginAsync(TokenType.Bot, _config["Token"]);
                await client.StartAsync();
                // sets the bots status indicator to "Do not disturb" if its still on cooldown
                if (File.Exists("db/lastmessage.37"))
                {
                    var last37 = Convert.ToDateTime(File.ReadAllText("db/lastmessage.37"));
                    TimeSpan ts = DateTime.UtcNow - last37;
                    if (ts.TotalMinutes < Int32.Parse(_config["Frequency"]))
                    {
                        Console.WriteLine("Cooldown Triggered");
                        Cooldown cooldown = new Cooldown();
                        cooldown.CooldownAsync((int)(int.Parse(_config["Frequency"])*1000*60 - ts.TotalMilliseconds), _client);
                    }
                }
                // we get the CommandHandler class here and call the InitializeAsync method to start things up for the CommandHandler service
                await services.GetRequiredService<CommandHandler>().InitializeAsync();
                

                await Task.Delay(-1);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        

        // this method handles the ServiceCollection creation/configuration, and builds out the service provider we can call on later
        private ServiceProvider ConfigureServices()
        {
            // this returns a ServiceProvider that is used later to call for those services
            // we can add types we have access to here, hence adding the new using statement:
            // using csharpi.Services;
            // the config we build is also added, which comes in handy for setting the command prefix!
            return new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }
        
    }
}
    