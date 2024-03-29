﻿using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Fortnite_API;

namespace BancoBahiaBot.Modules
{
    public class DebugCommands : ModuleBase<SocketCommandContext>
    {
        readonly Random random = new();

        [Command("Pergunta"), Alias("Question")]
        public async Task QuestionCommand([Remainder] string args)
        {
            bool chance = Convert.ToBoolean(random.Next(0, 2));
            string reply = $"{args} : {chance}\n\nPergunta feita por: {Context.User.Mention}!";

            await Context.Channel.SendMessageAsync(reply);
            Terminal.WriteLine($"Replying to {Context.User} ({Context.User.Id}):\n{reply}", Terminal.MessageType.INFO, ConsoleColor.Gray);
        }

        [Command("SaveLog")]
        public Task SaveLogCommand()
        {
            Terminal.SaveLog();

            return null;
        }

        // DEBUG (JUST FOR NIC :) )
        [Command("AddItem"), Alias("AdicionarItem")]
        public async Task AddItemCommand(string itemId, string quantity)
        {
            if (Context.User.Id != 345680337277288448) return;

            int quantityInt;
            try
            {
                quantityInt = int.Parse(quantity);
            }
            catch (Exception e)
            {
                Terminal.WriteLine($"Bot use error {e.Message} by {Context.User} ({Context.User.Id})", Terminal.MessageType.WARN);
                await Context.Channel.SendMessageAsync("Deve ser um numero inteiro!");
                return;
            }

            User user = UserHandler.GetUser(Context.User.Id);
            Item item = ItemHandler.GetItem(itemId);
            if (item == null)
            {
                await Context.Channel.SendMessageAsync("Este item não existe!");
                return;
            }

            ItemHandler.AddItemToUser(user, item, quantityInt);

            string reply = $"Adicionado {quantityInt} de {item.name} para o ADM :place_of_worship::place_of_worship::place_of_worship:!";

            await Context.Channel.SendMessageAsync(reply);

            Terminal.WriteLine($"Added {quantityInt} of {item.id} to {Context.User} ({Context.User.Id})", Terminal.MessageType.INFO);
        }

        // DEBUG (JUST FOR NIC :) )
        [Command("AddStock"), Alias("AdicionarAcao")]
        public async Task AddStockCommand(string stockId, string quantity)
        {
            if (Context.User.Id != 345680337277288448) return;

            int quantityInt;
            try
            {
                quantityInt = int.Parse(quantity);
            }
            catch (Exception e)
            {
                Terminal.WriteLine($"Bot use error {e.Message} by {Context.User} ({Context.User.Id})", Terminal.MessageType.WARN);
                await Context.Channel.SendMessageAsync("Deve ser um numero inteiro!");
                return;
            }

            User user = UserHandler.GetUser(Context.User.Id);
            Stock stock = StockHandler.GetStock(stockId);
            if (stock == null)
            {
                await Context.Channel.SendMessageAsync("Esse ticker não existe!");
                return;
            }

            StockHandler.AddStockToUser(user, stock, quantityInt);

            string reply = $"Adicionado {quantityInt} de {stock.name} ({stock.shortName}) para o ADM :place_of_worship::place_of_worship::place_of_worship:!";

            await Context.Channel.SendMessageAsync(reply);

            Terminal.WriteLine($"Added {quantityInt} stocks of {stock.id} to {Context.User} ({Context.User.Id})", Terminal.MessageType.INFO);
        }

        // DEBUG (JUST FOR NIC :) )
        [Command("SetStockPrice"), Alias("SetarPrecoAcao")]
        public async Task SetStockPriceCommand(string stockId, string quantity)
        {
            if (Context.User.Id != 345680337277288448) return;

            int quantityInt;
            try
            {
                quantityInt = int.Parse(quantity);
            }
            catch (Exception e)
            {
                Terminal.WriteLine($"Bot use error {e.Message} by {Context.User} ({Context.User.Id})", Terminal.MessageType.WARN);
                await Context.Channel.SendMessageAsync("Deve ser um numero inteiro!");
                return;
            }

            Stock stock = StockHandler.GetStock(stockId);
            if (stock == null)
            {
                await Context.Channel.SendMessageAsync("Esse ticker não existe!");
                return;
            }

            stock.price = quantityInt;

            string reply = $"Setado ${quantityInt} de preço em {stock.name} ({stock.shortName}) para o ADM :place_of_worship::place_of_worship::place_of_worship:!";

            await Context.Channel.SendMessageAsync(reply);

            Terminal.WriteLine($"Setted ${quantityInt} price of {stock.id} by {Context.User} ({Context.User.Id})", Terminal.MessageType.INFO);
        }

        // DEBUG (JUST FOR NIC :) )
        [Command("AddMoney"), Alias("AdicionarDinheiro")]
        public async Task AddMoneyCommand(string quantity)
        {
            if (Context.User.Id != 345680337277288448) return;

            int money;
            try
            {
                money = int.Parse(quantity);
            }
            catch (Exception e)
            {
                Terminal.WriteLine($"Bot use error {e.Message} by {Context.User} ({Context.User.Id})", Terminal.MessageType.WARN);
                await Context.Channel.SendMessageAsync("Deve ser um numero inteiro!");
                return;
            }

            UserHandler.GetUser(Context.User.Id).money += money;

            string reply = $"Adicionado {money} de dinheiro para o ADM :place_of_worship::place_of_worship::place_of_worship:!";

            await Context.Channel.SendMessageAsync(reply);

            Terminal.WriteLine($"Added {money} of money to {Context.User} ({Context.User.Id})", Terminal.MessageType.INFO);
        }

        // DEBUG (JUST FOR NIC :) )
        [Command("AddMoney"), Alias("AdicionarDinheiro")]
        public async Task AddMoneyCommand(IGuildUser mention, string quantity)
        {
            if (Context.User.Id != 345680337277288448) return;

            int money;
            try
            {
                money = int.Parse(quantity);
            }
            catch (Exception e)
            {
                Terminal.WriteLine($"Bot use error {e.Message} by {Context.User} ({Context.User.Id})", Terminal.MessageType.WARN);
                await Context.Channel.SendMessageAsync("Deve ser um numero inteiro!");
                return;
            }

            UserHandler.GetUser(mention.Id).money += money;

            string reply = $"Adicionado {money} de dinheiro para {mention.Mention}!";

            await Context.Channel.SendMessageAsync(reply);

            Terminal.WriteLine($"Added {money} of money to {mention} ({mention.Id}) by {Context.User} ({Context.User.Id})", Terminal.MessageType.INFO);
        }

        // DEBUG (JUST FOR NIC :) )
        [Command("ResetPropertiesTime")]
        public async Task ResetPropertiesTimeCommand()
        {
            if (Context.User.Id != 345680337277288448) return;

            UserProperty[] userProperties = UserHandler.GetUser(Context.User.Id).properties;

            foreach (UserProperty userProperty in userProperties)
                userProperty.lastCollect = userProperty.lastCollect.AddDays(-1);

            await Context.Channel.SendMessageAsync("Tempo de coleta de propriedades resetados para o ADM :place_of_worship::place_of_worship::place_of_worship:!");
        }

        [Command("UserMoneyRank")]
        public async Task UserMoneyRankCommand()
        {
            string usersString = string.Empty;
            User[] sortedUsers = UserHandler.GetUsers().OrderByDescending(x => x.money).ToArray();

            foreach (User user in sortedUsers)
            {
                if(user.money != 0)
                    usersString += $"{Bot.Client.GetUser(ulong.Parse(user.id))} : {user.money} \n";
            }

            await Context.Channel.SendMessageAsync(usersString);
        }

        [Command("reac")]
        public Task ReacCommand()
        {
            ReactionHandler.AddReactionRequest(async (IUser user, object param) =>
            {
                await Context.Channel.SendMessageAsync($"debug reac {param}, por: " + user.Mention);
            }, new("✅"), Context.Message, "dababy", true);
            
            return null;
        }

        [Command("btn")]
        public async Task BtnCommand()
        {
            var builder = new ComponentBuilder();

            builder.WithButton("yummy", "yummy-id");

            Bot.Client.ButtonExecuted += async (SocketMessageComponent button) =>
            {
                await button.RespondAsync($"{button.User.Mention} has clicked {button.Data.CustomId}");
            };

            await Context.Channel.SendMessageAsync("debug btn", components: builder.Build());
        }

        #region Fornite API

        FortniteApiClient fnApi = new();

        [Command("fn_s")]
        public async Task FortniteStatsCommand([Remainder]string nickname)
        {
            var stats = await fnApi.V2.Stats.GetBrV2Async(x =>
            {
                x.Name = nickname;
                x.ImagePlatform = Fortnite_API.Objects.V1.BrStatsV2V1ImagePlatform.All;
            });

            await Context.Channel.SendMessageAsync(stats.Data.Image.ToString());
        }

        [Command("fn_m")]
        public async Task FortniteMapCommand()
        {
            var map = await fnApi.V1.Map.GetAsync(Fortnite_API.Objects.GameLanguage.PT_BR);

            await Context.Channel.SendMessageAsync(map.Data.Images.POIs.ToString());
        }

        #endregion

        #region audio

        [Command("audio"), DoNotEnterTypingState]
        public async Task JoinChannel([Remainder]string music)
        {
            if(Context.User.Id != 345680337277288448) return;

            // Get the audio channel
            var channel = (Context.User as IGuildUser).VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel."); return; }
            
            // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
            var audioClient = await channel.ConnectAsync();
            
            await SendAsync(audioClient, $"Sounds/{music}.mp3");
        }

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -vol 8192 -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }

        private async Task SendAsync(IAudioClient client, string path)
        {
            // Create FFmpeg using the previous example
            using (var ffmpeg = CreateStream(path))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
            {
                try { await output.CopyToAsync(discord); }
                finally { await discord.FlushAsync(); ffmpeg.Close(); }
            }
        }

        #endregion
    }
}
