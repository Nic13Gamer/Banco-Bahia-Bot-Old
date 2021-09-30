using Discord.Commands;
using System.Threading.Tasks;

namespace BancoBahiaBot.Modules
{
    public class GuildCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Guild")]
        public async Task GuildCommand()
        {
            Guild guild = GuildHandler.GetGuild(Context.Guild.Id);

            await Context.Channel.SendMessageAsync($"{guild.discordGuild.Name} ({guild.id})");
        }
    }
}
