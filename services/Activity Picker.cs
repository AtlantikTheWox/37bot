using System;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace botof37s.services
{
    public class Activitypicker
    {
        public void Pick(DiscordSocketClient _client)
        {
            DirectoryInfo di = new DirectoryInfo("audio");
            var files = di.GetFiles();
            FileInfo song = files[new Random().Next(0, files.Length)];
            var tfile = TagLib.File.Create($"audio/{song.Name}");
            string title = tfile.Tag.Title;
            string artist = tfile.Tag.Performers.FirstOrDefault();
            List<string> activities = new List<string>
            {
                "l https://open.spotify.com/track/5qHlikfhP5fD5DUwTbJR9D?si=b2ekUgO9RsedV3uL1XdiOw",
                "w My mayo jar before Simmo gets to it",
                "w Harry Elshams sanity slowly deteriorate",
                $"p Fursonacon {DateTime.Now.Year} VR",
                "w https://www.youtube.com/watch?v=EJRXWNWJOrQ",
                $"p Simmo's Mod appreciation simulator {DateTime.Now.Year}",
                "s 37s@https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                "w Watt being appreciated ❤️",
                "w Bob spam",
                "p with my best friends Simbot and VentBot",
                "l Simbot's favourite song",
                "s Simbot's favourite song@https://www.youtube.com/watch?v=Mz3Mi_OZYno",
                $"l \"{title}\" by \"{artist}\""
                


            };
            if (File.Exists("db/activities.37"))
            {
                var activitiestxt = File.ReadAllLines("db/activities.37");
                for (int i = 0; i < activitiestxt.Length; i++)
                {
                    activities.Add(activitiestxt[i]);
                }
            }
            
            int selection = new Random().Next(0, activities.ToArray().Length);
            char activity = activities[selection].ToCharArray()[0];
            string streamlink = null;
            ActivityType activityType = ActivityType.Playing;
            string activitystring = null;
            switch (activity)
            {
                case 'l':
                    activityType = ActivityType.Listening;
                    activitystring = activities[selection].Remove(0, 2);
                    break;
                case 'w':
                    activityType = ActivityType.Watching;
                    activitystring = activities[selection].Remove(0, 2);
                    break;
                case 'p':
                    activityType = ActivityType.Playing;
                    activitystring = activities[selection].Remove(0, 2);
                    break;
                case 's':
                    activityType = ActivityType.Streaming;
                    streamlink = activities[selection].Split("@")[1];
                    activitystring = activities[selection].Remove(0, 2).Split("@")[0];
                    break;
            }
            _client.SetGameAsync(activitystring, streamlink, activityType);
        }
    }
}