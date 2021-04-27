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


    public class Activitycommand : ModuleBase
    {
        public IConfiguration _config { get; set; }
        public DiscordSocketClient _client { get; set; }
        public CommandHandler handler { get; set; }

        [Command("activity")]
        [Alias("status")]
        [Summary("Sets the activity of the bot")]
        [Name("💬 activity <reset|playing|listening|watching> (<activity>)")]
        [Remarks("authorized")]
        public async Task ActivitycommandAsync(string type = "reset", [Remainder] string activity = null)
        {
            
            Authorisationcheck authorisationcheck = new Authorisationcheck();
            if (!authorisationcheck.Check(Context.User.Id, _config))
                return;
            switch(type)
            {
                case "reset":
                    Activitypicker picker = new Activitypicker();
                    picker.Pick((DiscordSocketClient)Context.Client);
                    handler.customFalse();
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> The activity has been reset!");
                    break;
                case "playing":
                    if(activity == null)
                    {
                        await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> You need to provide an activity");
                        break;
                    }
                    await _client.SetGameAsync(activity, null, ActivityType.Playing);
                    handler.customTrue();
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> The activity has been sucessfully set to \"Playing {activity}\"!");
                    break;
                case "watching":
                    if (activity == null)
                    {
                        await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> You need to provide an activity");
                        break;
                    }
                    await _client.SetGameAsync(activity, null, ActivityType.Watching);
                    handler.customTrue();
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> The activity has been sucessfully set to \"Watching {activity}\"!");
                    break;
                case "listening":
                    if (activity == null)
                    {
                        await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> You need to provide an activity");
                        break;
                    }
                    if (activity.StartsWith("to "))
                        activity = activity.Remove(0, 3);
                    handler.customTrue();
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> The activity has been sucessfully set to \"Listening to {activity}\"!");
                    await _client.SetGameAsync(activity, null, ActivityType.Listening);
                    break;
                case "streaming":
                    if (activity == null)
                    {
                        await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> You need to provide an activity");
                        break;
                    }
                    try
                    {
                        _ = activity.Split("@")[1];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> I know the help commmand doesnt say it, but this activity is special. You need to provide the activity in the following syntax: \"<activity>@<yt or twitch link>\"");
                        break;
                    }
                    await _client.SetGameAsync(activity.Split("@")[0], activity.Split("@")[1].Replace("<","").Replace(">",""), ActivityType.Streaming);
                    handler.customTrue();
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> The activity has been sucessfully set to \"Streaming {activity.Split("@")[0]}\" with the link <{activity.Split("@")[1].Replace("<", "").Replace(">", "")}>!");
                    break;
                default:
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Invalid argument!");
                    return;
            }
            
        }
        [Command("addactivity")]
        [Alias("addstatus")]
        [Summary("Adds an activity to the default list the bot picks from when no custom activity is set")]
        [Name("➕ addactivity <playing|listening|watching> <activity>")]
        [Remarks("authorized")]
        public async Task AddactivitycommandAsync(string type = null, [Remainder] string activity = null)
        {
            Authorisationcheck authorisationcheck = new Authorisationcheck();
            if (!authorisationcheck.Check(Context.User.Id, _config))
                return;
            switch (type)
            {
                
                case "playing":
                    if (activity == null)
                    {
                        await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> You need to provide an activity");
                        break;
                    }
                    if (!File.Exists("db/activities.37"))
                    {
                        File.WriteAllText("db/activities.37", $"p {activity}");
                    }
                    else
                    {
                        var acts = File.ReadAllText("db/activities.37");
                        File.WriteAllText("db/activities.37", acts + $"\np {activity}");
                    }
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> The activity \"Playing {activity}\" has successfully been added!");
                    break;
                case "watching":
                    if (activity == null)
                    {
                        await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> You need to provide an activity");
                        break;
                    }
                    if (!File.Exists("db/activities.37"))
                    {
                        File.WriteAllText("db/activities.37", $"p {activity}");
                    }
                    else
                    {
                        var acts = File.ReadAllText("db/activities.37");
                        File.WriteAllText("db/activities.37", acts + $"\nw {activity}");
                    }
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> The activity \"Watching {activity}\" has successfully been added!");
                    break;
                case "listening":
                    if (activity == null)
                    {
                        await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> You need to provide an activity");
                        break;
                    }
                    if (activity.StartsWith("to "))
                        activity = activity.Remove(0, 3);
                    if (!File.Exists("db/activities.37"))
                    {
                        File.WriteAllText("db/activities.37", $"l {activity}");
                    }
                    else
                    {
                        var acts = File.ReadAllText("db/activities.37");
                        File.WriteAllText("db/activities.37", acts + $"\nl {activity}");
                    }
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> The activity \"Listening {activity}\" has successfully been added!");
                    break;
                default:
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Invalid argument!");
                    return;
            }

        }
    }
}