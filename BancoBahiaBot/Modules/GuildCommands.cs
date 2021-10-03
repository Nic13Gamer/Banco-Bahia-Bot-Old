using Discord;
using Discord.Commands;
using System.Collections;
using System.Threading.Tasks;

namespace BancoBahiaBot.Modules
{
    public class GuildCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Prefix"), Alias("Prefixo"), RequireUserPermission(GuildPermission.ManageGuild)]
        [CommandHelp(
                name: "prefix",
                alias: "prefixo",
                use: "prefix <novo prefixo>",
                description: "Muda o prefixo de comandos do servidor",
                permissions: new[] { GuildPermission.ManageGuild }
            )]
        public async Task PrefixCommand(string prefix)
        {
            Guild guild = GuildHandler.GetGuild(Context.Guild.Id);

            guild.prefix = prefix;
            await Context.Channel.SendMessageAsync($"O prefixo de {guild.discordGuild.Name} foi alterado para {guild.prefix}");
        }
    }
}
