using BancoBahiaBot.Utils;

using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BancoBahiaBot.Modules
{
    public class UserCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Profile"), Alias("Perfil")]
        [CommandHelp(
                name: "profile",
                aliases: "perfil",
                uses: "profile||{prefix}profile <usuário>",
                description: "Mostra o seu perfil ou o de outro usuário"
            )]
        public async Task ProfileCommand()
        {
            var content = new Dictionary<string, string>
            {
                { "profilePic", Context.User.GetAvatarUrl(size: 512) },
                { "username", Context.User.Username },
                { "money", UserHandler.GetUser(Context.User.Id).money.ToString() }
            };

            HttpResponse response = await NetUtils.BotApiRequest("profile", content);

            if(response.status != 200)
                return;

            await Context.Channel.SendFileAsync(response.content, $"Perfil de {Context.User.Mention}");
        }

        [Command("Profile"), Alias("Perfil")]
        public async Task ProfileCommand(IGuildUser mention)
        {
            if (mention.IsBot) return;

            var content = new Dictionary<string, string>
            {
                { "profilePic", mention.GetAvatarUrl(size: 512) },
                { "username", mention.Username },
                { "money", UserHandler.GetUser(mention.Id).money.ToString() }
            };

            HttpResponse response = await NetUtils.BotApiRequest("profile", content);

            if (response.status != 200)
                return;

            await Context.Channel.SendFileAsync(response.content, $"Perfil de {mention.Mention}");
        }
    }
}
