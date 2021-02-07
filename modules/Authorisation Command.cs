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

namespace botof37s.Modules
{


    public class Authorisationcommand : ModuleBase
    {

        [Command("authorized")]
        [Summary("Adds or removes a person to the list of authorized people")]
        [Name("👮 authorized <add|remove>")]
        [Remarks("owner")]
        [RequireOwner]
        public async Task AuthorizeCommand(string option = null, [Remainder] string args = null)
        {
            switch (option)
            {
                case "add":

                    break;
                case "remove":

                    break;
                default:
                    return;
            }
        }
    }
}