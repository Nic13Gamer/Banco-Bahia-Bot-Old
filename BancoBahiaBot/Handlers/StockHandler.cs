using BancoBahiaBot.Utils;

using System;
using System.Collections.Generic;
using System.Threading;

namespace BancoBahiaBot
{
    class StockHandler
    {
        static readonly Random random = new();

        static Thread stocksUpdaterThread;

        public static readonly List<Stock> stocks = new();

        #region Define stocks

        public static Stock ursinhusLTDA = new
            (
                id: "ursinhus_ltda",
                name: "Ursinhus LTDA",
                shortName: "URNL",
                price: 250
            );

        #endregion

        public static void Start()
        {
            stocks.Add(ursinhusLTDA);

            stocksUpdaterThread = new(StocksUpdater);
            stocksUpdaterThread.Start();

            Terminal.WriteLine("Stocks updater thread started successfully!");
        }

        static void StocksUpdater()
        {
            while (true)
            {
                foreach (Stock stock in stocks)
                {
                    int chance = 60;
                    int randomChance = random.Next(0, 101);

                    int newPrice = stock.price;

                    if (stock.wentUp)
                    {
                        if (randomChance <= chance)
                            newPrice += 5;
                        else
                        {
                            newPrice -= 5;
                            stock.wentUp = false;
                        }
                    }
                    else
                    {
                        if (randomChance <= chance)
                            newPrice -= 5;
                        else
                        {
                            newPrice += 5;
                            stock.wentUp = true;
                        }
                    }

                    newPrice = Math.Clamp(newPrice, 150, 1000);
                    stock.price = newPrice;

                    if (stock.lastPrices.Count >= 24)
                        stock.lastPrices.RemoveAt(0);

                    stock.lastPrices.Add(stock.price);
                }

                Thread.Sleep(1); // UPDATE EACH HOUR
            }
        }

        public static Stock GetStock(string stock)
        {
            stock = StringUtils.RemoveAccents(stock.ToLower());

            foreach (Stock _stock in stocks)
            {
                if (StringUtils.RemoveAccents(_stock.id.ToLower()) == stock || StringUtils.RemoveAccents(_stock.name.ToLower()) == stock
                    || StringUtils.RemoveAccents(_stock.shortName.ToLower()) == stock)
                {
                    return _stock;
                }
            }

            return null;
        }
    }

    class Stock
    {
        public Stock(string id, string name, string shortName, int price)
        {
            this.id = id;
            this.name = name;
            this.shortName = shortName;
            this.price = price;
        }

        public string id;
        public string name;
        public string shortName;
        public int price;

        public bool wentUp = true;

        public readonly List<int> lastPrices = new();
    }

    class UserStock
    {
        public UserStock(Stock stock, int quantity)
        {
            this.stock = stock;
            this.quantity = quantity;
        }

        public Stock stock;
        public int quantity;
    }
}
