using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BancoBahiaBot
{
    class ReactionHandler
    {
        public static void Start() => Bot.Client.ReactionAdded += ReactionAdded;

        static readonly List<ReactionRequest> pendingReactionRequests = new();

        private static Task ReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot) return null;

            foreach (ReactionRequest pendingReaction in pendingReactionRequests)
            {
                if (reaction.Emote.Name != pendingReaction.reaction.Name || reaction.MessageId != pendingReaction.message.Id) continue;

                pendingReaction.callback?.Invoke(reaction.User.Value, pendingReaction.param);

                pendingReactionRequests.Remove(pendingReaction);
                break;
            }

            return null;
        }

        public async static void AddReactionRequest(Action<IUser, object> callback, Emoji reaction, IMessage message, object param, bool addReaction = false)
        {
            ReactionRequest newPendingReaction = new
                (
                    callback: callback,
                    reaction: reaction,
                    message: message,
                    param: param
                );

            pendingReactionRequests.Add(newPendingReaction);

            if(addReaction)
            await message.AddReactionAsync(reaction);
        }

        struct ReactionRequest
        {
            public ReactionRequest(Action<IUser, object> callback, Emoji reaction, IMessage message, object param)
            {
                this.callback = callback;
                this.reaction = reaction;
                this.message = message;
                this.param = param;
            }

            public Action<IUser, object> callback;
            public Emoji reaction;
            public IMessage message;
            public object param;
        }
    }
}
