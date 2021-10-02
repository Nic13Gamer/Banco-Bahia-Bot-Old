using Discord;
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

        public CommandHandler()
        {
            client = Bot.Client;
            service = new CommandService();

            service.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            client.MessageReceived += HandleCommand;
        }

        Task HandleCommand(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return null;

            var context = new SocketCommandContext(client, msg);
            Guild guild = GuildHandler.CreateGuild(context.Guild.Id);

            int argPos = 0;
            if (msg.HasStringPrefix(guild.prefix, ref argPos))
            {
                if (context.User.IsBot || context.IsPrivate) return null;
                UserHandler.CreateUser(context.User.Id);
                
                Thread thread = new(async () =>
                {
                    var result = await service.ExecuteAsync(context, argPos, null);

                    if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    {
                        string reply = $"{context.User.Mention}, alguma coisa deu errado! Motivo: " + result.ErrorReason;

                        Embed commandHelp = HelpHandler.GetCommandHelpEmbed(context.Message.Content.Split(" ")[0].Replace(guild.prefix, "").Trim());
                        if (commandHelp != null) reply = context.User.Mention;

                        Terminal.WriteLine($"Bot use error [{result.ErrorReason}] by {context.User} ({context.User.Id})", Terminal.MessageType.WARN);

                        await context.Channel.SendMessageAsync(reply, embed: commandHelp);
                    }

                    SaveManager.SaveAll();
                });
                thread.Start();
            }

            return null;
        }
    }
}
