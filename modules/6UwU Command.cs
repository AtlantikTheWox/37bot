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
using Microsoft.Extensions.Configuration.Json;
using botof37s;
using colorpicker;
using System.Text.RegularExpressions;

namespace botof37s.Modules
{


    public class UwUfy : ModuleBase
    {
        IConfiguration _config { get; set; }
        [Command("uwufy", RunMode = RunMode.Async)]
        [Alias("owofy")]
        [Summary("uwufies the most recent message in a channel or a message requested")]
        [Name("<:uwu:819967959475814430> uwufy <-|message ID>")]
        [Remarks("all")]
        public async Task UwUfyCommand(string uwuid = null)
        {
            string uwu = "";
            IMessage messig = null;
            if (uwuid == null)
            {
                IEnumerable<IMessage> message = null;
                bool isBot = true;
                int count = 1;
                while (isBot)
                {
                    message = await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, count).FlattenAsync();
                    messig = message.LastOrDefault();

                    if (!messig.Author.IsBot)
                    {
                        if (messig.Content != null && messig.Content != "")
                        {
                            if (!messig.Content.StartsWith("/37 "))
                            {
                                isBot = false;
                            }
                        }
                    }

                    count++;
                }
                uwu = message.LastOrDefault().Content;
            }
            else
            {
                var guilds = ((DiscordSocketClient)Context.Client).Guilds;
                foreach (SocketGuild guild in guilds)
                {
                    foreach (IMessageChannel channel in guild.TextChannels)
                    {
                        try
                        {
                            messig = await channel.GetMessageAsync(ulong.Parse(uwuid));
                        }
                        catch (Exception)
                        {

                        }
                        if (messig != null)
                            goto stop;
                    }
                }
            stop:;
                if (messig == null)
                {
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> I'm sowwy, but i'm having twoubwe finding that message, please twy again >w<");
                    return;
                }
                uwu = messig.Content;

            }

            string[] faces = new string[] { "(・`ω´・)", ";;w;;", "owo", "UwU", ">w<", "^w^", "OwO", "UwU", "ÚwÚ", "^-^", ":3", "x3" };
            string[] actions = new string[] { @"\*blushes\*", @"\*whispers to self\*", @"\*cries\*", @"\*screams\*", @"\*sweats\*", @"\*twerks\*", @"\*runs away\*", @"\*screeches\*", @"\*walks away\*", @"\*sees bulge\*", @"\*looks at you\*", @"\*notices buldge\*", @"\*starts twerking\*", @"\*huggies tightly\*", @"\*boops your nose\*", @"\*calls Simmo a fuwwy\*" };
            uwu = Regex.Replace(uwu, "(?:r|l)", "w");
            uwu = Regex.Replace(uwu, "(?:R|L)", "W");
            uwu = Regex.Replace(uwu, "th([aeiouAEIOU])", "d$1");
            uwu = Regex.Replace(uwu, "Th([aeiouAEIOU])", "D$1");
            uwu = Regex.Replace(uwu, "TH([aeiouAEIOU])", "D$1");
            uwu = Regex.Replace(uwu, "na", "nya");
            uwu = Regex.Replace(uwu, "Na", "Nya");
            uwu = Regex.Replace(uwu, "NA", "NYA");
            uwu = Regex.Replace(uwu, "ove ", "uv");
            var words = uwu.Split(" ");
            for(int i = 0; i < words.Length;  i++)
            {

                if(new Random().Next(11) == 3)
                {
                    char start = words[i].ToCharArray()[0];
                    words[i] = start + "-" + words[i];
                }else if(new Random().Next(51) == 11)
                {
                    words[i] = faces[new Random().Next(faces.Length)] + " " + words[i];
                }else if(new Random().Next(101) == 69)
                {
                    words[i] = actions[new Random().Next(actions.Length)] + " " + words[i];
                }
            }
            uwu = string.Join(" ", words);
            uwu += $" {faces[new Random().Next(faces.Length)]}";
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithAuthor(messig.Author.Username + "#" + messig.Author.Discriminator, messig.Author.GetAvatarUrl());
            builder.WithDescription(uwu);
            Colorpicker picker = new Colorpicker();
            builder.WithColor(picker.Pick());
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

    }

}