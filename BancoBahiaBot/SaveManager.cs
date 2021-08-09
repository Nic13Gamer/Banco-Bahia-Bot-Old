using SimpleJSON;
using System;
using System.IO;

namespace BancoBahiaBot
{
    class SaveManager
    {
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
                File.WriteAllText(Bot.DATA_PATH + "/botData.json", json.ToString());
            }
            catch (Exception e)
            {
                Terminal.WriteLine(e.Message, Terminal.MessageType.ERROR);
            }
        }
    }
}
