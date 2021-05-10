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
            IMessage messig = null;
            string uwu = new string(string.Empty);
            if (uwuid == null)
            {

                IEnumerable<IMessage>  message = await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, 100).FlattenAsync();
                foreach(IMessage messag in message)
                {
                    if (!messag.Author.IsBot)
                    {
                        if (messag.Content != null && messag.Content != "")
                        {
                            if (!messag.Content.StartsWith("/37") && !messag.Content.StartsWith("!")&& !messag.Content.StartsWith("+"))
                            {
                                uwu = messag.Content;
                                break;
                            }
                        }
                    }
                }
                
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
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> I'm sowwy, but i'm having twoubwe finding dat message, please twy again >w<");
                    return;
                }
                uwu = messig.Content;

            }

            string[] faces = new string[] { "(・`ω´・)", ";;w;;", "owo", "UwU", ">w<", "^w^", "OwO", "UwU", "ÚwÚ", "^-^", ":3", "x3" };
            string[] actions = new string[] { @"\*blushes\*", @"\*whispers to self\*", @"\*cries\*", @"\*screams\*", @"\*sweats\*", @"\*runs away\*", @"\*screeches\*", @"\*walks away\*",  @"\*looks at you\*",  @"\*huggies tightly\*", @"\*boops your snoot\*" };
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
                }else if(new Random().Next(150) == 69)
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