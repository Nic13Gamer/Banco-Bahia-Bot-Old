using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace BancoBahiaBot.Modules
{
    public class InventoryCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Inventory"), Alias("Itens")]
        public async Task InventoryCommand()
        {
            User user = UserHandler.GetUser(Context.User.Id.ToString());
            UserItem[] userInventory = user.inventory;
            string items = string.Empty;

            foreach (UserItem userItem in userInventory)
            {
                items += $"**`{userItem.item.name}`** : {userItem.quantity}\n";
            }

            EmbedBuilder embed = new EmbedBuilder
            {
                Title = "**INVENTÁRIO**",
                Color = Color.Orange
            }.WithAuthor(Context.User).WithCurrentTimestamp().WithFooter(footer => footer.Text = "Para ver mais sobre um item, use ?item <nome>");

            if(items != string.Empty)
                embed.AddField("> Itens possuídos",
                    items);
            else
                embed.AddField("> Itens possuídos",
                    "`Você ainda não possui nenhum item`");

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }

        [Command("Inventory"), Alias("Itens")]
        public async Task InventoryCommand(IUser mention)
        {
            User user = UserHandler.GetUser(mention.Id.ToString());
            UserItem[] userInventory = user.inventory;
            string items = string.Empty;

            foreach (UserItem userItem in userInventory)
            {
                items += $"**`{userItem.item.name}`** : {userItem.quantity}\n";
            }

            EmbedBuilder embed = new EmbedBuilder
            {
                Title = "**INVENTÁRIO**",
                Color = Color.Orange
            }.WithAuthor(mention).WithCurrentTimestamp().WithFooter(footer => footer.Text = "Para ver mais sobre um item, use ?item <nome>");

            if (items != string.Empty)
                embed.AddField("> Itens possuídos",
                    items);
            else
                embed.AddField("> Itens possuídos",
                    $"`{mention.Username} ainda não possui nenhum item`");

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }

        [Command("SellItem"), Alias("VenderItem")]
        public async Task SellItemCommand([Remainder]string msg)
        {
            User user = UserHandler.GetUser(Context.User.Id.ToString());

            string[] args = msg.Split(" ");
            string itemString = string.Empty;
            int quantity = 1;

            foreach (string arg in args)
            {
                if(int.TryParse(arg, out int num))
                {
                    quantity = num;
                    continue;
                }

                itemString += arg + " ";
            }
            itemString = itemString.Trim();

            if(quantity < 1)
            {
                await Context.Channel.SendMessageAsync("Deve ser maior que 0!");
                return;
            }

            Item item = ItemHandler.GetItem(itemString);
            if (item == null)
            {
                await Context.Channel.SendMessageAsync("Este item não existe!");
                return;
            }
            UserItem userItem = ItemHandler.GetUserItem(item, UserHandler.GetUser(Context.User.Id.ToString()));
            if (userItem == null)
            {
                await Context.Channel.SendMessageAsync("Você não possui este item!");
                return;
            }
            if(userItem.quantity < quantity)
            {
                await Context.Channel.SendMessageAsync("Você não tem itens suficientes!");
                return;
            }

            int sellPrice = userItem.item.sellPrice;

            ItemHandler.RemoveItemFromUser(user, userItem.item, quantity);
            user.money += sellPrice * quantity;

            await Context.Channel.SendMessageAsync($"Você vendeu {quantity} de {userItem.item.name} por {sellPrice * quantity}!");

            Terminal.WriteLine($"{Context.User} ({Context.User.Id}) sold {quantity} of {userItem.item.id} for {sellPrice * quantity}");
        }
    }
}
