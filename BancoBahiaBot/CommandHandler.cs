using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
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

            client.MessageReceived += HandleCommandAsync;
        }

        async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;

            var context = new SocketCommandContext(client, msg);

            int argPos = 0;
            if (msg.HasCharPrefix('?', ref argPos))
            {
                if (context.User.IsBot) return;

                UserHandler.CreateUser(context.User.Id.ToString());
                var result = await service.ExecuteAsync(context, argPos, null);
                UserHandler.SaveUsersData();

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    string reply = "Alguma coisa deu errado! Motivo: " + result.ErrorReason;

                    if (result.Error == CommandError.BadArgCount)
                        reply = context.Message + " tem poucos ou muitos argumentos!";

                    Terminal.WriteLine($"Bot use error {result.Error} by {context.User} ({context.User.Id})", Terminal.MessageType.WARN);

                    await context.Channel.SendMessageAsync(reply);
                }
            }
        }
    }
}
