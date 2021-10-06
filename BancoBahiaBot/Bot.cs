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

        public static readonly SaveManager.BotOptions BotOptions = SaveManager.LoadBotOptions();

        public static readonly string API_KEY = BotOptions.apiKey;

        public static DiscordSocketClient Client { get; private set; }

        CommandHandler commandHandler;

        public async Task StartBot()
        {
            Console.Title = "Bot Banco Bahia";

            Client = new DiscordSocketClient();

            await Client.LoginAsync(TokenType.Bot, BotOptions.token);
            await Client.StartAsync();

            await Task.Delay(1500);

            commandHandler = new();

            ItemHandler.Start();
            PropertyHandler.Start();
            StockHandler.Start();

            SaveManager.LoadAll();

            ReactionHandler.Start();

            Terminal.Start();

            Terminal.WriteLine("Bot started successfully!", Terminal.MessageType.INFO);

            await Client.SetGameAsync("Sou um banco que tem seu próprio dinheiro virtual e muito mais!");

            await Task.Delay(-1);
        }
    }
}
