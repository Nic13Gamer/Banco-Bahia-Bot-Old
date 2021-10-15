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
        public async Task ProfileCommand(IGuildUser mention)
        {
            var user = (IUser)mention ?? Context.User;
            if (user.IsBot) return;

            var content = new Dictionary<string, string>
            {
                { "profilePic", user.GetAvatarUrl(size: 512) },
                { "username", user.Username },
                { "money", UserHandler.GetUser(user.Id).money.ToString() }
            };

            HttpResponse response = await NetUtils.BotApiRequest("profile", content);

            if(response.status != 200)
                return;

            await Context.Channel.SendFileAsync(response.content, $"Perfil de {user.Mention}");
        }
    }
}
