using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Http;
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


    public class Wheelcommand : ModuleBase
    {
        public IConfiguration _config { get; set; }
        public DiscordSocketClient _client { get; set; }
        public CommandHandler handler { get; set; }

        [Command("wheel",RunMode = RunMode.Async)]
        [Alias("roll")]
        [Summary("Spins the wheel (DM only)")]
        [Name("🎰 wheel <help|token>")]
        [Remarks("authorized")]
        public async Task Wheelspoof(string token = null)
        {
            Authorisationcheck authorisationcheck = new Authorisationcheck();
            if (!authorisationcheck.Check(Context.User.Id, _config)) return;
            if(Context.Channel.GetType().ToString() != "Discord.WebSocket.SocketDMChannel") 
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Due to the sensitive nature of this command it is resticted to dms only");
                await Context.Message.DeleteAsync();
                return;
            }
            if(token == null)
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> You need to provide your token. If you don't know how to get your token use \"help\" instead of it to get a tutorial.");
                return;
            }
            if(token.ToLower() == "help")
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> To get your token go to <https://gamblingbot.app>, sign in with Discord, press f12, go to the \"Application\" tab, click on \"Local Storage\", then \"https://gamblingbot.app/\" and then copy the value for token");
                return;
            }
            HttpClient client = new HttpClient();
            Dictionary<int, int> distrib = new Dictionary<int, int>();
            client.DefaultRequestHeaders.Add("Accept", "application / json, text / plain, */*");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Host", "gamblingbot.app");
            client.DefaultRequestHeaders.Add("Pragma", "no-cache");
            client.DefaultRequestHeaders.Add("Referer", "https://gamblingbot.app/games/fortune-wheel");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0");
            DateTime start = DateTime.UtcNow;
            bool mil = false;
            bool startconfirmation = false;
            while (!mil)
            {
                HttpResponseMessage response = await client.GetAsync($"https://gamblingbot.app/api/games/fortune-wheel-start");
                string responsestring = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responsestring);
                if (responsestring == "invalid user")
                {
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> The allmighty bot didnt like that one, pls try again:");
                    return;
                }
                if (responsestring == "Insufficient amount of gems")
                {
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Your spin appears to still be on cooldown, please try again later");
                    return;
                }
                if (!startconfirmation)
                {
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> I have started up the automatic spin feature for you. Be advised that this may take upwards of multiple hours on some days.");
                    startconfirmation = true;
                }
                if (responsestring == "{\"win\":0}")
                {
                    mil = true;
                    await Task.Delay(7000);
                }
                try
                {
                    _ = int.Parse(responsestring.Replace("{\"win\":", "").Replace("}", ""));
                }
                catch (FormatException)
                {
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Oops, i didnt expect the following response: \"{responsestring}\" Please contact my owner with this information or try again");
                    return;
                }
                await Task.Delay(5000 + new Random().Next(-1000, 1000));
            }
            HttpResponseMessage winresponse = await client.GetAsync($"https://gamblingbot.app/api/games/fortune-wheel-ok");
            TimeSpan ts = DateTime.UtcNow - start;
            await Context.Channel.SendMessageAsync($"Success! After {ts.Hours} hours, {ts.Minutes} minutes and {ts.Seconds} seconds i have managed to make a winning roll! Raw response: \"{await winresponse.Content.ReadAsStringAsync()}\"");
        }
    }
}