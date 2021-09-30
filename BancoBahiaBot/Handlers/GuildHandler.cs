using Discord.WebSocket;
using System.Collections.Generic;

namespace BancoBahiaBot
{
    class GuildHandler
    {
        static readonly List<Guild> guilds = new();

        public static Guild GetGuild(ulong id) => CreateGuild(id);

        public static Guild CreateGuild(ulong id)
        {
            SocketGuild discordGuild = Bot.Client.GetGuild(id);
            if (discordGuild == null) throw new System.Exception("Guild in discord does not exist!");

            foreach (Guild _guild in guilds)
                if (_guild.id == id.ToString()) return _guild;

            Guild guild = new
                (
                    id: id.ToString(),
                    discordGuild: discordGuild
                );

            guilds.Add(guild);
            return guild;
        }

        public static Guild[] GetGuilds() => guilds.ToArray();
    }

    class Guild
    {
        public Guild(string id, SocketGuild discordGuild)
        {
            this.id = id;
            this.discordGuild = discordGuild;
        }

        public string id;
        public SocketGuild discordGuild;
    }
}
