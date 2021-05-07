using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
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

        [Command("wheel", RunMode = RunMode.Async)]
        [Alias("roll")]
        [Summary("Spins the wheel (DM only)")]
        [Name("🎰 wheel <help|token>")]
        [Remarks("authorized")]
        public async Task Wheelspoof(string token = null)
        {
            Authorisationcheck authorisationcheck = new Authorisationcheck();
            if (!authorisationcheck.Check(Context.User.Id, _config)) return;
            if (Context.Channel.GetType().ToString() != "Discord.WebSocket.SocketDMChannel")
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Due to the sensitive nature of this command it is resticted to dms only");
                await Context.Message.DeleteAsync();
                return;
            }
            if (File.Exists($"wheelspoof/tokens/{Context.User.Id}.37"))
            {
                token = File.ReadAllText($"wheelspoof/tokens/{Context.User.Id}.37");
            }
            if (token == null)
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> You need to provide your token. If you don't know how to get your token use \"help\" instead of it to get a tutorial.");
                return;
            }
            if (token.ToLower() == "help")
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}>This tutorial only works on desktop. To get your token go to <https://gamblingbot.app> with Chrome or Firefox, sign in with Discord, press f12, go to the \"Application\" tab, click on \"Local Storage\", then \"https://gamblingbot.app/\" and then copy the value for token");
                return;
            }
            if (File.Exists($"wheelspoof/{token}"))
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> This token is currently already being used. If you think this is a mistake contact my owner");
                return;
            }
            DateTime lastreminder = new DateTime();
            await File.WriteAllTextAsync($"wheelspoof/{token}","uwu");
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
            if(await cooldownresponse.Content.ReadAsStringAsync() == "invalid user")
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> The allmighty bot didnt like that one. This may be because your token has expired or because the gambling bot is having technical difficulties.");
                Console.WriteLine("Invalid free cooldown");
                if (File.Exists($"wheelspoof/{token}")) File.Delete($"wheelspoof/{token}");
                if (File.Exists($"wheelspoof/tokens/{Context.User.Id}.37")) File.Delete($"wheelspoof/tokens/{Context.User.Id}.37");
                return;
            }
            else
            {
                DateTimeOffset unixstart = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(await cooldownresponse.Content.ReadAsStringAsync()));
                unixstart = unixstart.AddHours(22);
                if (DateTime.UtcNow.CompareTo(unixstart.UtcDateTime) < 0)
                {
                    TimeSpan cooldown = unixstart - DateTime.UtcNow;
                    string oncooldownresponse = $"<@{Context.User.Id}> Your spin is still on cooldown for {cooldown.Hours} hours, {cooldown.Minutes} minutes and {cooldown.Seconds} seconds. Please try again then.";
                    if (!File.Exists($"wheelspoof/tokens/{Context.User.Id}.37") || File.ReadAllText($"wheelspoof/tokens/{Context.User.Id}.37") != token)
                    {
                        File.WriteAllText($"wheelspoof/tokens/{Context.User.Id}.37", token);
                        oncooldownresponse += " I have saved you token for you, which means you wont need to provide it next time";
                    }
                    if (File.Exists($"wheelspoof/{token}")) File.Delete($"wheelspoof/{token}");
                    await Context.Channel.SendMessageAsync(oncooldownresponse);
                    return;
                }
                
            }
            while (!mil)
            {
                HttpResponseMessage response = await client.GetAsync($"https://gamblingbot.app/api/games/fortune-wheel-start");
                string responsestring = await response.Content.ReadAsStringAsync();
                if (responsestring == "invalid user")
                {
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> The allmighty bot didnt like that one. This may be because your token has expired or because the gambling bot is having technical difficulties.");
                    if(File.Exists($"wheelspoof/{token}")) File.Delete($"wheelspoof/{token}");
                    return;
                }
                if (!startconfirmation)
                {
                    string confirmationresponse = $"<@{Context.User.Id}> I have started up the automatic spin feature for you. Be advised that this may take upwards of multiple hours on some days. I will keep you updated around every 15 minutes.";
                    startconfirmation = true;
                    lastreminder = DateTime.Now;
                    if (!File.Exists($"wheelspoof/tokens/{Context.User.Id}.37")||File.ReadAllText($"wheelspoof/tokens/{Context.User.Id}.37") != token)
                    {
                        File.WriteAllText($"wheelspoof/tokens/{Context.User.Id}.37", token);
                        responsestring += " I have also saved the token for you. If you want to roll with the same token next time you dont have to provide it again";
                    }
                    await Context.Channel.SendMessageAsync(confirmationresponse);
                }
                try
                {
                    int key = int.Parse(responsestring.Replace("{\"win\":", "").Replace("}", ""));
                    distrib[key]++;
                }
                catch (FormatException)
                {
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Oops, i didnt expect the following response: \"{responsestring}\" Please contact my owner with this information or try again");
                    File.Delete($"wheelspoof/{token}");
                    return;
                }
                if((DateTime.Now - lastreminder).TotalMinutes > 15)
                {
                    int counter = 0;
                    foreach(KeyValuePair<int,int> kvp in distrib)
                    {
                        counter += kvp.Value;
                    }
                    TimeSpan temp = DateTime.UtcNow - start;
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Still rolling! I have been rolling for {temp.Hours} hours, {temp.Minutes} minutes and {temp.Seconds} seconds and missed {counter} times so far :(");
                    lastreminder = DateTime.Now;
                }
                if (responsestring == "{\"win\":0}")
                {
                    mil = true;
                    await Task.Delay(7000);
                }
                
                await Task.Delay(5000 + new Random().Next(-1000, 1000));
            }
            HttpResponseMessage winresponse = await client.GetAsync($"https://gamblingbot.app/api/games/fortune-wheel-ok");
            TimeSpan ts = DateTime.UtcNow - start;
            WebClient web = new WebClient();
            Stream stream = web.OpenRead($"https://quickchart.io/chart?bkg=white&c={{type:'bar',data:{{labels:['10,000,000','9,000','45,000','11,500','450,000','15,000','60,000','20,000','250,000','30,000','120,000','6,000'],datasets:[{{ label:'Times rolled',data:[{distrib[0]},{distrib[1]},{distrib[2]},{distrib[3]},1{distrib[4]},{distrib[5]},{distrib[6]},{distrib[7]},{distrib[8]},{distrib[9]},{distrib[10]},{distrib[11]}]}}]}}}}");
            Bitmap bitmap = new Bitmap(stream);
            bitmap.Save($"wheelspoof/barcharts/{Context.Message.Id}.png", System.Drawing.Imaging.ImageFormat.Png);
            File.Delete($"wheelspoof/{token}");
            await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Success! After {ts.Hours} hours, {ts.Minutes} minutes and {ts.Seconds} seconds i have managed to make a winning roll! Raw response: \"{await winresponse.Content.ReadAsStringAsync()}\"  Distribution chart: ");
            await Context.Channel.SendFileAsync($"wheelspoof/barcharts/{Context.Message.Id}.png");
            File.Delete($"wheelspoof/barcharts/{Context.Message.Id}.png");
        }
    }
}