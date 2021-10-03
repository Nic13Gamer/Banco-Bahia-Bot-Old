using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
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
                    IDisposable typingState = null;

                    if(service.Search(context, argPos).Commands != null)
                        typingState = context.Channel.EnterTypingState();

                    var result = await service.ExecuteAsync(context, argPos, null);

                    if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    {
                        string reply = $"{context.User.Mention}, alguma coisa deu errado! Motivo: " + result.ErrorReason;

                        Embed commandHelp = HelpHandler.GetCommandHelpEmbed(service.Search(context, argPos).Commands[0].Command.Name, guild);
                        if (commandHelp != null) reply = context.User.Mention;

                        Terminal.WriteLine($"Bot use error [{result.ErrorReason}] by {context.User} ({context.User.Id})", Terminal.MessageType.WARN);

                        await context.Channel.SendMessageAsync(reply, embed: commandHelp);
                    }

                    if(typingState != null)
                        typingState.Dispose();

                    SaveManager.SaveAll();
                });
                thread.Start();
            }

            return null;
        }
    }
}
