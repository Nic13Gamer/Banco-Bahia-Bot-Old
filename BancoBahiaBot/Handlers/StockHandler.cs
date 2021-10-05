using BancoBahiaBot.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BancoBahiaBot
{
    class StockHandler
    {
        static readonly Random random = new();

        static Thread stocksUpdaterThread;

        static readonly List<Stock> stocks = new();

        #region Define stocks

        public static Stock ursinhusLTDA = new
            (
                id: "ursinhusLtda",
                name: "Ursinhus LTDA",
                shortName: "URNL",
                price: 240
            );

        public static Stock menezesCompany = new
            (
                id: "menezesCompany",
                name: "Menezes Company",
                shortName: "MZCY",
                price: 480
            );

        public static Stock bancoBahiaInc = new
            (
                id: "bancoBahiaInc",
                name: "Banco Bahia Inc",
                shortName: "BCBI",
                price: 430
            );

        public static Stock nikosCompany = new
            (
                id: "nikosCompany",
                name: "Nikos Company",
                shortName: "NKSC",
                price: 280
            );

        public static Stock lyonStateInc = new
            (
                id: "lyonStateInc",
                name: "Lyon State Inc",
                shortName: "LYSI",
                price: 340
            );

        public static Stock lipezSportsCompany = new
            (
                id: "lipezSportsCompany",
                name: "Lipez Sports Company",
                shortName: "LZSC",
                price: 390
            );

        public static Stock joteiElectronicsInc = new
            (
                id: "joteiElectronicsInc",
                name: "Jotei Electronics Inc",
                shortName: "JTCI",
                price: 410
            );

        public static Stock monoWavesLtda = new
            (
                id: "monoWavesLtda",
                name: "MonoWaves LTDA",
                shortName: "MWVL",
                price: 370
            );

        public static Stock brocolisCompany = new
            (
                id: "brocolisCompany",
                name: "Brocolis Company",
                shortName: "BCLC",
                price: 420
            );

        public static Stock brucesInc = new
            (
                id: "brucesInc",
                name: "Bruces Inc",
                shortName: "BRCI",
                price: 210
            );

        #endregion

        public static void Start()
        {
            stocks.Add(ursinhusLTDA);
            stocks.Add(menezesCompany);
            stocks.Add(bancoBahiaInc);
            stocks.Add(nikosCompany);
            stocks.Add(lyonStateInc);
            stocks.Add(lipezSportsCompany);
            stocks.Add(joteiElectronicsInc);
            stocks.Add(monoWavesLtda);
            stocks.Add(brocolisCompany);
            stocks.Add(brucesInc);

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
                    int chance = 65 + random.Next(-4, 5);
                    int randomChance = random.Next(0, 101);

                    int modifier = random.Next(38, 88);

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

                    if (stock.lastPrices.Count >= 46)
                        stock.lastPrices.RemoveAt(0);

                    stock.lastPrices.Add(stock.price);
                }

                Thread.Sleep(300000); // 5 minutes

                SaveManager.SaveAll();
            }
        }

        public static Stock[] GetStocks() => stocks.ToArray();

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

        public static UserStock GetUserStock(string stock, User user)
        {
            Stock chosenStock = GetStock(stock);

            if (chosenStock == null)
                return null;

            UserStock userStock = null;
            foreach (UserStock _userStock in user.stocks)
            {
                if (_userStock.stock.id == chosenStock.id)
                {
                    userStock = _userStock;
                    break;
                }
            }

            return userStock;
        }

        public static void AddStockToUser(User user, Stock stock, int quantity = 1)
        {
            List<UserStock> userStocks = user.stocks.ToList();

            foreach (UserStock userStock in userStocks)
            {
                if (userStock.stock.id == stock.id)
                {
                    if (stock.price > userStock.highBuyPrice || userStock.highBuyPrice == -1)
                        userStock.highBuyPrice = stock.price;

                    userStock.quantity += quantity;
                    user.stocks = userStocks.ToArray();

                    return;
                }
            }

            UserStock newUserStock = new(stock, quantity);
            if(newUserStock.highBuyPrice == -1)
                newUserStock.highBuyPrice = stock.price;

            userStocks.Add(newUserStock);

            user.stocks = userStocks.ToArray();
        }

        public static void RemoveStockFromUser(User user, Stock stock, int quantity = 1)
        {
            List<UserStock> userStocks = user.stocks.ToList();

            foreach (UserStock userStock in user.stocks)
            {
                if (userStock.stock.id == stock.id)
                {
                    userStock.quantity -= quantity;

                    if (userStock.quantity <= 0)
                    {
                        userStocks.Remove(userStock);
                    }
                }
            }

            user.stocks = userStocks.ToArray();
        }
    }

    class Stock
    {
        public Stock(string id, string name, string shortName, int price/*, int stocksUpdatePayback*/)
        {
            this.id = id;
            this.name = name;
            this.shortName = shortName;
            this.price = price;
            //this.stocksUpdatePayback = stocksUpdatePayback;
        }

        public string id;
        public string name;
        public string shortName;
        public int price;

        //public int stocksUpdatePayback;
        public bool wentUp = true;

        public List<int> lastPrices = new();
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
        public int highBuyPrice = -1;
    }
}
