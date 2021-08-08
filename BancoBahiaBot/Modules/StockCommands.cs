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
                        string data = string.Empty;
                        string stockString = StringUtils.GetAllRemainderTextAfter(args, 0);

                        Stock stock = StockHandler.GetStock(stockString);
                        if(stock == null)
                        {
                            await Context.Channel.SendMessageAsync("Essa ação não existe!");
                            return;
                        }

                        foreach (int price in stock.lastPrices)
                        {
                            if (data == string.Empty)
                                data += price;
                            else
                                data += "," + price;
                        }

                        EmbedBuilder embed = new EmbedBuilder
                        {
                            Title = $"**{stock.name}**",
                            Color = Color.Orange,
                            Url = $"http://localhost:5500/stocks/chart.html?stock={stock.shortName}&data={data}"
                        }.WithCurrentTimestamp();

                        await Context.Channel.SendMessageAsync(embed: embed.Build());

                        return;
                    }

                case "buystock" or "compraracao":
                    {
                        // buy stocks if possible

                        break;
                    }

                case "sellstock" or "venderacao":
                    {
                        // sell stocks if possible

                        break;
                    }
            }
        }

        [Command("Broker")]
        public async Task BrokerCommand()
        {
            // show stock prices
        }
    }
}
