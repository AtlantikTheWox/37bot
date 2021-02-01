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

namespace botof37s.Services
{
    public class Threeseven
    {
        DateTime last37;
        private  IConfiguration _config;
        public async Task Command(SocketMessage message)
        {
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");


            _config = _builder.Build();
            if (File.Exists("db/lastmessage.37"))
            {
                last37 = Convert.ToDateTime(File.ReadAllText("db/lastmessage"));
            }
            TimeSpan ts = DateTime.UtcNow - last37;
            if (ts.TotalMinutes >= Int32.Parse(_config["Frequency"]))
            {

            }
        }
    }

}