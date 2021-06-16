using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace BancoBahiaBot
{
    class ReactionHandler
    {
        public static DiscordSocketClient client;

        public static void Start() => client.ReactionAdded += ReactionAdded;

        static async Task ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            
        }
    }
}
