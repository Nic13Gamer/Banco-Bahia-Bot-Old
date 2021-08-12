using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BancoBahiaBot
{
    class CommandHandler
    {
        readonly DiscordSocketClient client;
        readonly CommandService service;

        public CommandHandler(DiscordSocketClient _client)
        {
            client = _client;
            service = new CommandService();

            service.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            client.MessageReceived += HandleCommand;
        }

        Task HandleCommand(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return null;

            var context = new SocketCommandContext(client, msg);

            int argPos = 0;
            if (msg.HasStringPrefix("?", ref argPos))
            {
                if (context.User.IsBot || context.IsPrivate) return null;

                UserHandler.CreateUser(context.User.Id.ToString());
                
                Thread thread = new(async () =>
                {
                    var result = await service.ExecuteAsync(context, argPos, null);

                    if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    {
                        string reply = "Alguma coisa deu errado! Motivo: " + result.ErrorReason;

                        if (result.Error == CommandError.BadArgCount)
                            reply = context.Message + " tem poucos ou muitos argumentos!";

                        Terminal.WriteLine($"Bot use error [{result.ErrorReason}] by {context.User} ({context.User.Id})", Terminal.MessageType.WARN);

                        await context.Channel.SendMessageAsync(reply);
                    }
                });
                thread.Start();
            }

            return null;
        }
    }
}
