using BancoBahiaBot.Utils;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace BancoBahiaBot.Modules
{
    public class StockCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Broker")]
        public async Task BrokerCommand([Remainder] string msg)
        {
            string[] args = StringUtils.RemoveAccents(msg.ToLower()).Split(" ");

            switch (args[0])
            {
                case "chart":
                    {
                        string stockString = StringUtils.GetAllRemainderTextAfter(args, 0);

                        Stock stock = StockHandler.GetStock(stockString);
                        if(stock == null)
                        {
                            await Context.Channel.SendMessageAsync("Esse ticker não existe!");
                            return;
                        }

                        string data = GetStockLastPricesString(stock);

                        EmbedBuilder embed = new EmbedBuilder
                        {
                            Title = $"**{stock.name}**",
                            Color = Color.Orange,
                            Url = $"http://localhost:5500/stocks/chart.html?stock={stock.shortName}&data={data}"
                        }.WithCurrentTimestamp().WithFooter(footer => { footer.Text = $"Gráfico das últimas 48 horas de {stock.shortName}"; });

                        await Context.Channel.SendMessageAsync(Context.User.Mention, embed: embed.Build());

                        break;
                    }

                case "portifolio":
                    {
                        // show aquired stocks

                        break;
                    }

                case "buy" or "comprar":
                    {
                        // buy stocks if possible

                        break;
                    }

                case "sell" or "vender":
                    {
                        // sell stocks if possible

                        break;
                    }

                case "info":
                    {
                        // show information about a certain ticker

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
                Color = Color.Orange
            }.WithCurrentTimestamp().WithFooter(footer => { footer.Text = "Para ver mais sobre um ticker, use ?broker info <ticker>"; });


            foreach (Stock stock in StockHandler.stocks)
            {
                string chartData = GetStockLastPricesString(stock);
                string wentUpEmoji = stock.wentUp ? ":arrow_up:" : ":arrow_down:";

                embed.AddField($"{stock.name} `({stock.shortName})` {wentUpEmoji}",
                    $"Preço: **`${stock.price}`**\n[Gráfico de preços](http://localhost:5500/stocks/chart.html?stock={stock.shortName}&data={chartData})", true);
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
