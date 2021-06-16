using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace BancoBahiaBot
{
    class Bot
    {
        DiscordSocketClient client;

        CommandHandler commandHandler;

        public async Task StartBot()
        {
            Console.Title = "Bot Banco Bahia";

            client = new DiscordSocketClient();

            await client.LoginAsync(TokenType.Bot, "Nzc3Njc5NTk2OTQ1OTMyMjg4.X7G8kA.dPZ4MF3Uc5r-p0nzoBwn3CqljeI");
            await client.StartAsync();

            commandHandler = new CommandHandler(client);

            PropertyHandler.Start();

            UserHandler.client = client;
            UserHandler.LoadUsersData();

            ReactionHandler.client = client;
            ReactionHandler.Start();

            Terminal.WriteLine("Bot started successfully!", Terminal.MessageType.INFO);

            await Task.Delay(-1);
        }
    }
}
