﻿using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace BancoBahiaBot.Modules
{
    public class MoneyCommands : ModuleBase<SocketCommandContext>
    {
        readonly Random random = new();

        [Command("Atm"), Alias("Money")]
        [CommandHelp(
                name: "atm",
                aliases: "money",
                uses: "atm||{prefix}atm <usuário>",
                description: "Mostra o seu dinheiro ou o de outro usuário"
            )]
        public async Task AtmCommand(IGuildUser mention = null)
        {
            var discordUser = (IUser)mention ?? Context.User;
            if (discordUser.IsBot) return;

            User user = UserHandler.GetUser(discordUser.Id);
            string reply = $"{discordUser.Mention} tem {user.money} de dinheiro!";

            await Context.Channel.SendMessageAsync(reply);
        }

        [Command("Pay"), Alias("Pagar")]
        [CommandHelp(
                name: "pay",
                aliases: "pagar",
                uses: "pay <usuário> <quantidade>",
                description: "Transfira a quantidade escolhida de dinheiro para outro usuário"
            )]
        public async Task PayCommand(IGuildUser mention, string quantity)
        {
            if (mention.IsBot || mention == Context.User) return;

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
            if(money <= 0)
            {
                await Context.Channel.SendMessageAsync("A quantia mínima é 1 de dinheiro!");
                return;
            }

            User user = UserHandler.GetUser(Context.User.Id);
            User mentionUser = UserHandler.GetUser(mention.Id);

            string reply = $"{quantity} de dinheiro foi transferido para {mention.Mention}!";

            if(user.money < money)
                reply = $"{Context.User.Mention}, você não tem dinheiro suficiente!";
            else
            {
                user.money -= money;
                mentionUser.money += money;

                Terminal.WriteLine($"{Context.User} ({Context.User.Id}) transfered {money} to {mention} ({mention.Id})", Terminal.MessageType.INFO);
            }

            await Context.Channel.SendMessageAsync(reply);
        }


        [Command("Steal"), Alias("Roubar")]
        [CommandHelp(
                name: "steal",
                aliases: "roubar",
                uses: "steal <usuário>",
                description: "Tente roubar dinheiro de outro usuário, mas tome cuidado com a polícia :oncoming_police_car:"
            )]
        public async Task StealCommand(IGuildUser mention)
        {
            if (mention.IsBot || mention == Context.User) return;

            if (UserHandler.GetUser(Context.User.Id).money < 2000)
            {
                await Context.Channel.SendMessageAsync("Você deve ter no mínimo 2000 de dinheiro para poder roubar alguém!");
                return;
            }

            int money = random.Next(120, 1000);
            bool success = Convert.ToBoolean(random.Next(0, 2));

            var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} tentou roubar {money} de {mention.Mention}!");

            await Task.Delay(3000);

            if (UserHandler.GetUser(mention.Id).money < money)
            {
                await msg.ModifyAsync(x => x.Content = $"{mention.Mention} não tinha {money} de dinheiro, então, o roubo foi cancelado.");
                return;
            }

            if (success)
            {
                UserHandler.GetUser(Context.User.Id).money += money;
                UserHandler.GetUser(mention.Id).money -= money;
                
                await msg.ModifyAsync(x => x.Content = $"{Context.User.Mention} conseguiu roubar {money} de {mention.Mention}! {Context.User.Mention} ganhou {money}.");
                return;
            }
            else
            {
                UserHandler.GetUser(Context.User.Id).money -= money * 2;
                UserHandler.GetUser(mention.Id).money += money * 2;

                await msg.ModifyAsync(x => x.Content = $"{Context.User.Mention} fracassou o roubo e teve que pagar uma multa do dobro do dinheiro roubado! {mention.Mention} ganhou {money * 2}.");
            }
        }

        [Command("Daily")]
        [CommandHelp(
                name: "daily",
                aliases: "",
                uses: "daily",
                description: "Colete seu dinheiro diário"
            )]
        public async Task DailyCommand()
        {
            User user = UserHandler.GetUser(Context.User.Id);
            DateTime lastDaily = user.lastDaily;
            string reply;
            
            if (lastDaily.AddDays(1) <= DateTime.Now) 
            {
                int money = random.Next(1500, 5001);
                reply = $"Você ganhou {money} de dinheiro diário!";

                Terminal.WriteLine($"Added {money} of daily money to {Context.User} ({Context.User.Id})", Terminal.MessageType.INFO);
                user.money += money;
                user.lastDaily = DateTime.Now;
            }
            else
            {
                string remaining = (lastDaily.AddDays(1) - DateTime.Now).ToString();
                remaining = remaining.Substring(0, remaining.LastIndexOf("."));
                reply = $"Seu daily ainda não está pronto! Tempo restante: {remaining}.";
            }

            await Context.Channel.SendMessageAsync(reply);
        }

        /*struct TransferRequest
        {
            public TransferRequest()
            {

            }

            public IGuildUser from;
            public IGuildUser to;
            public int money;

            public bool fromAccepted;
            public bool toAccepted;
        }*/
    }
}
