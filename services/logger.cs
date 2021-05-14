using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace botof37s.services
{
    public class LogService
    {
        public LogService()
        {
            if (!Directory.Exists("logs")) Directory.CreateDirectory("logs"); 
        }

        public async Task LogAsync(string log,LogLevel severity = LogLevel.Default,ICommandContext Context = null)
        {
            string filename = "undefined.txt";
            switch (severity)
            {
                case LogLevel.Default:
                    filename = "Log.log";
                    break;
                case LogLevel.Error:
                    filename = "ErrorLog.log";
                    break;
                case LogLevel.Severe:
                    filename = "ExceptionLog.log";
                    break;
                default:
                    break;
            }
           
            string LogText = DateTime.UtcNow.ToString() + "-";
            if (Context != null)
            {
                string guild = "None";
                if (Context.Guild != null) guild = Context.Guild.Name;
                LogText += $"User: \"{Context.User.Username}#{Context.User.Discriminator}\" Channel: \"{Context.Channel.Name}\" Server: \"{guild}\" Command Message: \"{Context.Message.Content}\" -";
            }
            LogText += log;
            LogText = LogText.Replace("\n", "").Replace("\r", "");
            List<string> logfile = new List<string>();
            if (File.Exists($"logs/{filename}")) logfile.AddRange(await File.ReadAllLinesAsync($"logs/{filename}"));
            logfile.Add(LogText);
            await File.WriteAllLinesAsync($"logs/{filename}", logfile);
            return;

        }
    }
    public enum LogLevel
    {
        Default = 0,
        Error = 1,
        Severe = 2
    }
}
