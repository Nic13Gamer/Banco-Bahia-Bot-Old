using BancoBahiaBot.Utils;

using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace BancoBahiaBot.Modules
{
    public class StockCommands : ModuleBase<SocketCommandContext>
    {
        static bool isStocksOpen = true;

        [Command("Broker")]
        public async Task BrokerCommand([Remainder] string msg)
        {
            User user = UserHandler.GetUser(Context.User.Id);
            string[] args = StringUtils.RemoveAccents(msg.ToLower()).Split(" ");

            switch (args[0])
            {

                case "portifolio" or "portfolio":
                    {
                        EmbedBuilder embed = new EmbedBuilder
                        {
                            Title = $"**PORTIFÓLIO**",
                            Description = "A bolsa atualiza a cada 5 minutos.",
                            Color = Color.Orange
                        }.WithAuthor(Context.User).WithCurrentTimestamp().WithFooter(footer => { footer.Text = "Para ver mais sobre um ticker, use ?broker info <ticker>"; });


                        foreach (UserStock userStock in user.stocks)
                        {
                            Stock stock = userStock.stock;

                            string chartData = GetStockLastPricesString(stock);
                            string wentUpEmoji = stock.wentUp ? ":arrow_up:" : ":arrow_down:";
                            string stockSuccessEmoji = stock.price * userStock.quantity > userStock.highBuyPrice * userStock.quantity ? ":green_circle:" : ":red_circle:";

                            string totalString = $"Total: **`${stock.price * userStock.quantity}`**" +
                                $" | **`{(stock.price * userStock.quantity) - (userStock.highBuyPrice * userStock.quantity)}`** de ganho";

                            embed.AddField($"{stock.name} `({stock.shortName})` {wentUpEmoji} | {stockSuccessEmoji}",
                                totalString +
                                $"\nMaior preço de compra: **`${userStock.highBuyPrice}`** | **`${userStock.highBuyPrice * userStock.quantity}`**" +
                                $"\nQuantidade: **`{userStock.quantity}`**", true);
                        }

                        await Context.Channel.SendMessageAsync(Context.User.Mention, embed: embed.Build());

                        break;
                    }

                case "buy" or "comprar":
                    {
                        if (!isStocksOpen)
                        {
                            await Context.Channel.SendMessageAsync("A bolsa de valores não está aberta agora!");
                            return;
                        }

                        string[] buyArgs = StringUtils.GetAllRemainderTextAfter(args, 0).Split(" ");
                        string stockString = string.Empty;
                        int quantity = 1;

                        foreach (string arg in buyArgs)
                        {
                            if (int.TryParse(arg, out int num))
                            {
                                quantity = num;
                                continue;
                            }

                            stockString += arg + " ";
                        }
                        stockString = stockString.Trim();

                        if (quantity < 1)
                        {
                            await Context.Channel.SendMessageAsync("Deve ser maior que 0!");
                            return;
                        }

                        Stock stock = StockHandler.GetStock(stockString);
                        if (stock == null)
                        {
                            await Context.Channel.SendMessageAsync("Esse ticker não existe!");
                            return;
                        }

                        int totalPrice = stock.price * quantity;
                        if(user.money < totalPrice)
                        {
                            await Context.Channel.SendMessageAsync("Você não tem dinheiro suficiente!");
                            return;
                        }

                        user.money -= totalPrice;
                        StockHandler.AddStockToUser(user, stock, quantity);

                        string reply = $"Você comprou {quantity} ações de `{stock.name} ({stock.shortName})` por `${totalPrice}`!";
                        if (quantity == 1)
                            reply = $"Você comprou {quantity} ação de `{stock.name} ({stock.shortName})` por `${totalPrice}`!";

                        await Context.Channel.SendMessageAsync(reply);
                        Terminal.WriteLine($"{Context.User} ({Context.User.Id}) bought {quantity} {stock.id} for ${totalPrice}!", Terminal.MessageType.INFO);

                        break;
                    }

                case "sell" or "vender":
                    {
                        if (!isStocksOpen)
                        {
                            await Context.Channel.SendMessageAsync("A bolsa de valores não está aberta agora!");
                            return;
                        }

                        string[] buyArgs = StringUtils.GetAllRemainderTextAfter(args, 0).Split(" ");
                        string stockString = string.Empty;
                        int quantity = 1;

                        foreach (string arg in buyArgs)
                        {
                            if (int.TryParse(arg, out int num))
                            {
                                quantity = num;
                                continue;
                            }

                            stockString += arg + " ";
                        }
                        stockString = stockString.Trim();

                        if (quantity < 1)
                        {
                            await Context.Channel.SendMessageAsync("Deve ser maior que 0!");
                            return;
                        }

                        Stock stock = StockHandler.GetStock(stockString);
                        if (stock == null)
                        {
                            await Context.Channel.SendMessageAsync("Esse ticker não existe!");
                            return;
                        }
                        UserStock userStock = StockHandler.GetUserStock(stockString, user);
                        if (userStock == null)
                        {
                            await Context.Channel.SendMessageAsync("Você não possui essa ação!");
                            return;
                        }
                        if(userStock.quantity < quantity)
                        {
                            await Context.Channel.SendMessageAsync("Você não possui ações suficientes!");
                            return;
                        }

                        int totalPrice = stock.price * quantity;

                        user.money += totalPrice;
                        StockHandler.RemoveStockFromUser(user, stock, quantity);

                        string reply = $"Você vendeu {quantity} ações de `{stock.name} ({stock.shortName})` por `${totalPrice}`!";
                        if (quantity == 1)
                            reply = $"Você vendeu {quantity} ação de `{stock.name} ({stock.shortName})` por `${totalPrice}`!";

                        await Context.Channel.SendMessageAsync(reply);
                        Terminal.WriteLine($"{Context.User} ({Context.User.Id}) sold {quantity} {stock.id} for ${totalPrice}!", Terminal.MessageType.INFO);

                        break;
                    }

                case "info":
                    {
                        // show information about a certain ticker
                        // show chart about a certain ticker

                        break;
                    }
            }
        }

        [Command("Broker")]
        public async Task BrokerCommand()
        {
            EmbedBuilder embed = new EmbedBuilder
            {
                Title = $"**TICKERS DISPONÍVEIS**",
                Description = "A bolsa atualiza a cada 5 minutos.",
                Color = Color.Orange
            }.WithCurrentTimestamp().WithFooter(footer => { footer.Text = "Para ver mais sobre um ticker, use ?broker info <ticker>"; });

            foreach (Stock stock in StockHandler.stocks)
            {
                string chartData = GetStockLastPricesString(stock);
                string wentUpEmoji = stock.wentUp ? ":arrow_up:" : ":arrow_down:";

                embed.AddField($"{stock.name} `({stock.shortName})` {wentUpEmoji}",
                    $"Preço: **`${stock.price}`**", true);
            }

            await Context.Channel.SendMessageAsync(Context.User.Mention, embed: embed.Build());
        }

        static string GetStockLastPricesString(Stock stock)
        {
            string data = string.Empty;
            foreach (int price in stock.lastPrices)
            {
                if (data == string.Empty)
                    data += price;
                else
                    data += "," + price;
            }

            return data;
        }
    }
}
