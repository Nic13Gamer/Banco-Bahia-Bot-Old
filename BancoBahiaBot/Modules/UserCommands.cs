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
                alias: "perfil",
                uses: "profile||{prefix}profile <usuário>",
                description: "Mostra o seu perfil ou o de outro usuário"
            )]
        public async Task ProfileCommand()
        {
            var content = new Dictionary<string, string>
            {
                { "profilePic", Context.User.GetAvatarUrl() },
                { "username", Context.User.Username },
                { "money", UserHandler.GetUser(Context.User.Id).money.ToString() }
            };

            HttpResponse response = await NetUtils.ApiRequest(Bot.WEBSITE_API + $"/profile", content);

            if(response.status != 200)
            {
                await Context.Channel.SendMessageAsync(response.content + $" | Por: {Context.User.Mention}");
                return;
            }

            await Context.Channel.SendFileAsync(response.content, "Por: " + Context.User.Mention);
        }

        [Command("Profile"), Alias("Perfil")]
        public async Task ProfileCommand(IUser mention)
        {
            if (mention.IsBot) return;

            var content = new Dictionary<string, string>
            {
                { "profilePic", mention.GetAvatarUrl() },
                { "username", mention.Username },
                { "money", UserHandler.GetUser(mention.Id).money.ToString() }
            };

            HttpResponse response = await NetUtils.ApiRequest(Bot.WEBSITE_API + $"/profile", content);

            if (response.status != 200)
            {
                await Context.Channel.SendMessageAsync(response.content + $" | Por: {Context.User.Mention}");
                return;
            }

            await Context.Channel.SendFileAsync(response.content, "Por: " + Context.User.Mention);
        }
    }
}
