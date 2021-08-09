﻿using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BancoBahiaBot
{
    class Bot
    {
        public static readonly string DATA_PATH = "C:/Users/nicho/Desktop/Banco Bahia/Data"; //Directory.GetCurrentDirectory() + "/Data";  THIS IS A OVERRIDE FOR DEVELOPMENT

        DiscordSocketClient client;

        CommandHandler commandHandler;

        public async Task StartBot()
        {
            Console.Title = "Bot Banco Bahia";

            client = new DiscordSocketClient();

            await client.LoginAsync(TokenType.Bot, File.ReadAllText(DATA_PATH + "/token.txt").Trim());
            await client.StartAsync();

            commandHandler = new CommandHandler(client);

            ItemHandler.Start();
            PropertyHandler.Start();
            StockHandler.Start();

            SaveManager.Load();

            ReactionHandler.client = client;
            ReactionHandler.Start();

            Terminal.WriteLine("Bot started successfully!", Terminal.MessageType.INFO);

            await client.SetGameAsync("Sou um banco que tem seu próprio dinheiro virtual e muito mais!");

            await Task.Delay(-1);
        }
    }
}
