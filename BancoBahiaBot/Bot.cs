using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace BancoBahiaBot
{
    class Bot
    {
        public static readonly string DATA_PATH = "C:/Users/nicho/Desktop/Banco Bahia/Data"; //Directory.GetCurrentDirectory() + "/Data";  THIS IS A OVERRIDE FOR DEVELOPMENT
        public static readonly string WEBSITE = "http://localhost:5500"; // THIS IS A OVERRIDE FOR DEVELOPMENT
        public static readonly string WEBSITE_API = "http://localhost:3000"; // THIS IS A OVERRIDE FOR DEVELOPMENT

        static readonly SaveManager.BotOptions botOptions = SaveManager.LoadBotOptions();

        public static readonly string API_KEY = botOptions.apiKey;

        DiscordSocketClient client;

        CommandHandler commandHandler;

        public async Task StartBot()
        {
            Console.Title = "Bot Banco Bahia";
            
            client = new DiscordSocketClient();

            await client.LoginAsync(TokenType.Bot, botOptions.token);
            await client.StartAsync();

            commandHandler = new CommandHandler(client, botOptions.prefix);

            ItemHandler.Start();
            PropertyHandler.Start();
            StockHandler.Start();

            SaveManager.Load();

            ReactionHandler.Start(client);

            Terminal.WriteLine("Bot started successfully!", Terminal.MessageType.INFO);

            await client.SetGameAsync("Sou um banco que tem seu próprio dinheiro virtual e muito mais!");

            await Task.Delay(-1);
        }
    }
}
