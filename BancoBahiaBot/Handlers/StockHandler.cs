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
                id: "ursinhusLtda",
                name: "Ursinhus LTDA",
                shortName: "URNL",
                price: 250
            );

        public static Stock menezesCompany = new
            (
                id: "menezesCompany",
                name: "Menezes Company",
                shortName: "MZCY",
                price: 340
            );

        public static Stock menezesCompany1 = new
            (
                id: "menezesCompany1",
                name: "Menezes Company",
                shortName: "MZCY",
                price: 340
            );

        public static Stock menezesCompany2 = new
            (
                id: "menezesCompany2",
                name: "Menezes Company",
                shortName: "MZCY",
                price: 340
            );

        public static Stock menezesCompany3 = new
            (
                id: "menezesCompany3",
                name: "Menezes Company",
                shortName: "MZCY",
                price: 340
            );

        public static Stock menezesCompany4 = new
            (
                id: "menezesCompany4",
                name: "Menezes Company",
                shortName: "MZCY",
                price: 340
            );

        #endregion

        public static void Start()
        {
            stocks.Add(ursinhusLTDA);
            stocks.Add(menezesCompany);
            stocks.Add(menezesCompany1);
            stocks.Add(menezesCompany2);
            stocks.Add(menezesCompany3);
            stocks.Add(menezesCompany4);

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
                    int chance = 65 + random.Next(-5, 6);
                    int randomChance = random.Next(0, 101);

                    int modifier = random.Next(6, 13);

                    int newPrice = stock.price;

                    if (stock.wentUp)
                    {
                        if (randomChance <= chance)
                            newPrice += modifier;
                        else
                        {
                            newPrice -= modifier;
                            stock.wentUp = false;
                        }
                    }
                    else
                    {
                        if (randomChance <= chance)
                            newPrice -= modifier;
                        else
                        {
                            newPrice += modifier;
                            stock.wentUp = true;
                        }
                    }

                    newPrice = Math.Clamp(newPrice, 150, 10000);
                    stock.price = newPrice;

                    if (stock.lastPrices.Count >= 48)
                        stock.lastPrices.RemoveAt(0);

                    stock.lastPrices.Add(stock.price);
                }

                //SaveManager.SaveAll(); COMMENTED FOR PERFORMENCE REASONS

                Thread.Sleep(50); // UPDATE EACH HOUR
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
