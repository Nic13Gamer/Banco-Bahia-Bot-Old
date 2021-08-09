using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace BancoBahiaBot.Modules
{
    public class DebugCommands : ModuleBase<SocketCommandContext>
    {
        readonly Random random = new();

        [Command("Pergunta"), Alias("Question")]
        public async Task QuestionCommand([Remainder]string args)
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

            User user = UserHandler.GetUser(Context.User.Id.ToString());
            Item item = ItemHandler.GetItem(itemId);
            if (item == null)
            {
                await Context.Channel.SendMessageAsync("Este item não existe!");
                return;
            }

            UserItem userItem = new
                (
                    item: item,
                    quantity: quantityInt
                );

            ItemHandler.AddItemToUser(user, userItem);

            string reply = $"Adicionado {quantityInt} de {item.name} para o ADM :place_of_worship::place_of_worship::place_of_worship:!";

            await Context.Channel.SendMessageAsync(reply);

            Terminal.WriteLine($"Added {quantityInt} of {item.id} to {Context.User} ({Context.User.Id})", Terminal.MessageType.INFO);
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

            UserHandler.GetUser(Context.User.Id.ToString()).money += money;

            string reply = $"Adicionado {money} de dinheiro para o ADM :place_of_worship::place_of_worship::place_of_worship:!";

            await Context.Channel.SendMessageAsync(reply);

            Terminal.WriteLine($"Added {money} of money to {Context.User} ({Context.User.Id})", Terminal.MessageType.INFO);
        }

        // DEBUG (JUST FOR NIC :) )
        [Command("ResetPropertiesTime")]
        public async Task ResetPropertiesTimeCommand()
        {
            if (Context.User.Id != 345680337277288448) return;

            UserProperty[] userProperties = UserHandler.GetUser(Context.User.Id.ToString()).properties;

            foreach (UserProperty userProperty in userProperties)
                userProperty.lastCollect = userProperty.lastCollect.AddDays(-1);

            await Context.Channel.SendMessageAsync("Tempo de coleta de propriedades resatados para o ADM :place_of_worship::place_of_worship::place_of_worship:!");
        }

        [Command("reac")]
        public Task ReacCommand()
        {
            ReactionHandler.AddReactionRequest(Callback, new("✅"), Context.Message, "dababy", true);

            return null;
        }

        async void Callback(IUser user, object param)
        {
            await Context.Channel.SendMessageAsync($"debug reac {param}, por: " + user.Mention);
        }
    }
}
