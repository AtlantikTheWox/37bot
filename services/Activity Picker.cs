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
    public class Activitypicker
    {
        public void Pick(DiscordSocketClient _client)
        {
            Random rnd = new Random();
            int num = rnd.Next(0, 11);
            if (num == 0) _client.SetGameAsync("https://open.spotify.com/track/5qHlikfhP5fD5DUwTbJR9D?si=b2ekUgO9RsedV3uL1XdiOw", null, ActivityType.Listening);
            if (num == 1) _client.SetGameAsync("Bob being obsessed with Momo", null, ActivityType.Watching);
            if (num == 2) _client.SetGameAsync("my mayo jar before Simmo gets to it", null, ActivityType.Watching);
            if (num == 3) _client.SetGameAsync("Harry Elsham's sanity slowly deteriorate", null, ActivityType.Watching);
            if (num == 4) _client.SetGameAsync("Fursonacon 2020 VR", null, ActivityType.Playing);
            if (num == 5) _client.SetGameAsync("https://www.youtube.com/watch?v=EJRXWNWJOrQ", null, ActivityType.Watching);
            if (num == 6) _client.SetGameAsync("Simmo's Mod appreciation simulator 2020", null, ActivityType.Playing);
            if (num == 7) _client.SetGameAsync("37s", "https://www.youtube.com/watch?v=dQw4w9WgXcQ", ActivityType.Streaming);
            if (num == 8) _client.SetGameAsync("Watt being appreciated ❤️﻿", null, ActivityType.Watching);
            if (num == 9) _client.SetGameAsync("Bob spam﻿", null, ActivityType.Watching);
            if (num == 10) _client.SetGameAsync("with my best friends Simbot and VentBot﻿", null, ActivityType.Playing);
        }
    }
}