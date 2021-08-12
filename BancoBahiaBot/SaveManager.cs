using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BancoBahiaBot
{
    class SaveManager
    {
        static int autosaveSecondsInterval = 7;
        static Thread autosaveThread = null;

        static readonly string botDataJsonPath = Bot.DATA_PATH + "/botData.json";

        public static void SaveAll()
        {
            JSONObject json = new();

            #region Add users to JSON

            JSONObject usersJson = new();

            foreach (User user in UserHandler.GetUsers())
            {
                JSONObject userJson = new();

                userJson.Add("id", user.id);
                userJson.Add("money", user.money);
                userJson.Add("lastDaily", user.lastDaily.ToString());

                #region Save properties

                JSONObject userProperties = new();

                foreach (UserProperty property in user.properties)
                {
                    JSONObject userProperty = new();

                    userProperty.Add("id", property.property.id);
                    userProperty.Add("lastCollect", property.lastCollect.ToString());

                    userProperties.Add(property.property.id, userProperty);
                }

                userJson.Add("properties", userProperties);

                #endregion

                #region Save inventory

                JSONObject userInventory = new();

                foreach (UserItem item in user.inventory)
                {
                    JSONObject userItem = new();

                    userItem.Add("id", item.item.id);
                    userItem.Add("quantity", item.quantity);

                    userInventory.Add(item.item.id, userItem);
                }

                userJson.Add("inventory", userInventory);

                #endregion

                #region Save stocks

                JSONObject userStocks = new();

                foreach (UserStock stock in user.stocks)
                {
                    JSONObject userStock = new();

                    userStock.Add("id", stock.stock.id);
                    userStock.Add("quantity", stock.quantity);
                    userStock.Add("highBuyPrice", stock.highBuyPrice);

                    userStocks.Add(stock.stock.id, userStock);
                }

                userJson.Add("stocks", userStocks);

                #endregion

                usersJson.Add(user.id, userJson);
            }

            json.Add("users", usersJson);

            #endregion

            #region Add stocks to JSON

            JSONObject stocksJson = new();

            foreach (Stock stock in StockHandler.stocks)
            {
                JSONObject stockJson = new();

                stockJson.Add("id", stock.id);
                stockJson.Add("price", stock.price);
                stockJson.Add("wentUp", stock.wentUp);

                #region Save last prices

                JSONArray stockLastPrices = new();

                foreach (int price in stock.lastPrices)
                    stockLastPrices.Add(price);

                stockJson.Add("lastPrices", stockLastPrices);

                #endregion

                stocksJson.Add(stock.id, stockJson);
            }

            json.Add("stocks", stocksJson);

            #endregion

            try
            {
                File.WriteAllText(botDataJsonPath, json.ToString());
            }
            catch (Exception e)
            {
                Terminal.WriteLine(e.Message, Terminal.MessageType.ERROR);
            }
        }

        public static void Load()
        {
            if (!File.Exists(botDataJsonPath)) File.Create(botDataJsonPath);
            string rawJson = File.ReadAllText(botDataJsonPath); if (rawJson.Trim() == string.Empty) { SaveAll(); rawJson = File.ReadAllText(botDataJsonPath); }
            JSONObject json = (JSONObject)JSON.Parse(rawJson);

            foreach (JSONObject user in json["users"])
                LoadUserFromJson(user);

            foreach (JSONObject stock in json["stocks"])
                LoadStockFromJson(stock);

            if(autosaveThread == null)
            {
                autosaveThread = new(Autosave);
                autosaveThread.Start();
            }
        }

        static void LoadUserFromJson(JSONObject userJson)
        {
            try
            {
                User newUser = UserHandler.CreateUser(userJson["id"]);

                newUser.money = userJson["money"];
                newUser.lastDaily = DateTime.Parse(userJson["lastDaily"]);

                #region Load properties

                List<UserProperty> properties = new();

                foreach (JSONObject userPropertyJson in userJson["properties"])
                {
                    Property property = PropertyHandler.GetProperty(userPropertyJson["id"]);

                    UserProperty userProperty = new(
                            property,
                            DateTime.Parse(userPropertyJson["lastCollect"])
                        );

                    properties.Add(userProperty);
                }

                newUser.properties = properties.ToArray();

                #endregion

                #region Load inventory

                List<UserItem> inventory = new();

                foreach (JSONObject userItemJson in userJson["inventory"])
                {
                    Item item = ItemHandler.GetItem(userItemJson["id"]);

                    UserItem userItem = new
                        (
                            item,
                            quantity: int.Parse(userItemJson["quantity"])
                        );

                    inventory.Add(userItem);
                }

                newUser.inventory = inventory.ToArray();

                #endregion

                #region Load stocks

                List<UserStock> stocks = new();

                foreach (JSONObject userStockJson in userJson["stocks"])
                {
                    Stock stock = StockHandler.GetStock(userStockJson["id"]);

                    UserStock userStock = new
                        (
                            stock,
                            quantity: userStockJson["quantity"]
                        );
                    userStock.highBuyPrice = userStockJson["highBuyPrice"];

                    stocks.Add(userStock);
                }

                newUser.stocks = stocks.ToArray();

                #endregion
            }
            catch (Exception e)
            {
                Terminal.WriteLine($"Error while loading user data: {e.Message} | User: {userJson["id"]}", Terminal.MessageType.ERROR);
                return;
            }

        }

        static void LoadStockFromJson(JSONObject stockJson)
        {
            try
            {
                Stock stock = StockHandler.GetStock(stockJson["id"]);

                stock.price = stockJson["price"];
                stock.wentUp = stockJson["wentUp"];

                foreach (JSONNumber price in stockJson["lastPrices"])
                    stock.lastPrices.Add(price);
            }
            catch (Exception e)
            {
                Terminal.WriteLine($"Error while loading stock data: {e.Message} | Stock: {stockJson["id"]}", Terminal.MessageType.ERROR);
                return;
            }
        }


        static void Autosave()
        {
            while (true)
            {
                SaveAll();

                Thread.Sleep(autosaveSecondsInterval * 1000);
            }
        }
    }
}
