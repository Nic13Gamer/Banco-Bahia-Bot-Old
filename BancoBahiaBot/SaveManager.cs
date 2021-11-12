using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace BancoBahiaBot
{
    public abstract class SaveManager
    {
        static readonly string botDataJsonPath = Bot.DATA_PATH + "/botData.json";

        public static void SaveAll()
        {
            BotData data = new();

            #region Save users

            List<SaveUser> saveUsers = new();

            foreach (User user in UserHandler.GetUsers())
            {
                SaveUser saveUser = new(user.id, user.money, user.lastDaily);

                #region Save properties

                List<SaveUser.SaveUserProperty> properties = new();
                foreach (UserProperty property in user.properties)
                {
                    properties.Add(new(property.property.id, property.lastCollect));
                }

                saveUser.properties = properties.ToArray();

                #endregion

                #region Save items

                List<SaveUser.SaveUserItem> items = new();
                foreach (UserItem item in user.items)
                {
                    items.Add(new(item.item.id, item.quantity));
                }

                saveUser.items = items.ToArray();

                #endregion

                #region Save stocks

                List<SaveUser.SaveUserStock> stocks = new();
                foreach (UserStock stock in user.stocks)
                {
                    stocks.Add(new(stock.stock.id, stock.quantity, stock.highBuyPrice));
                }

                saveUser.stocks = stocks.ToArray();

                #endregion

                saveUsers.Add(saveUser);
            }

            data.users = saveUsers.ToArray();

            #endregion

            #region Save stocks

            List<SaveStock> saveStocks = new();

            foreach (Stock stock in StockHandler.GetStocks())
            {
                saveStocks.Add(new(stock.id, stock.price, stock.wentUp, stock.lastPrices.ToArray()));
            }

            data.stocks = saveStocks.ToArray();

            #endregion

            #region Save guilds

            List<SaveGuild> saveGuilds = new();

            foreach (Guild guild in GuildHandler.GetGuilds())
            {
                saveGuilds.Add(new(guild.id, guild.prefix));
            }

            data.guilds = saveGuilds.ToArray();

            #endregion

            string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);

            try
            {
                File.WriteAllText(botDataJsonPath, jsonString);
            }
            catch (Exception e)
            {
                Terminal.WriteLine(e.Message, Terminal.MessageType.ERROR);
            }
        }

        public static void LoadAll()
        {
            if (!File.Exists(botDataJsonPath)) File.Create(botDataJsonPath);
            string json = File.ReadAllText(botDataJsonPath); if (json.Trim() == string.Empty) { SaveAll(); json = File.ReadAllText(botDataJsonPath); }

            BotData data = JsonConvert.DeserializeObject<BotData>(json);

            #region Load users

            foreach (SaveUser saveUser in data.users)
            {
                try
                {
                    User user = UserHandler.CreateUser(ulong.Parse(saveUser.id));

                    user.money = saveUser.money;
                    user.lastDaily = saveUser.lastDaily;

                    #region Load properties

                    List<UserProperty> properties = new();
                    foreach (SaveUser.SaveUserProperty property in saveUser.properties)
                    {
                        properties.Add(new(PropertyHandler.GetProperty(property.id), property.lastCollect));
                    }

                    user.properties = properties.ToArray();

                    #endregion

                    #region Load items

                    List<UserItem> items = new();
                    foreach (SaveUser.SaveUserItem item in saveUser.items)
                    {
                        items.Add(new(ItemHandler.GetItem(item.id), item.quantity));
                    }

                    user.items = items.ToArray();

                    #endregion

                    #region Load stocks

                    List<UserStock> stocks = new();
                    foreach (SaveUser.SaveUserStock stock in saveUser.stocks)
                    {
                        UserStock userStock = new(StockHandler.GetStock(stock.id), stock.quantity);

                        userStock.highBuyPrice = stock.highBuyPrice;
                        stocks.Add(userStock);
                    }

                    user.stocks = stocks.ToArray();

                    #endregion
                }
                catch (Exception e)
                {
                    Terminal.WriteLine($"Error while loading user ({saveUser.id}): {e.Message}");
                }
            }

            #endregion

            #region Load stocks

            foreach (SaveStock saveStock in data.stocks)
            {
                try
                {
                    Stock stock = StockHandler.GetStock(saveStock.id);

                    stock.price = saveStock.price;
                    stock.wentUp = saveStock.wentUp;
                    stock.lastPrices = saveStock.lastPrices.ToList();
                }
                catch (Exception e)
                {
                    Terminal.WriteLine($"Error while loading stock ({saveStock.id}): {e.Message}");
                }
            }

            #endregion

            #region Load guilds

            foreach (SaveGuild saveGuild in data.guilds)
            {
                try
                {
                    Guild guild = GuildHandler.CreateGuild(ulong.Parse(saveGuild.id));

                    guild.prefix = saveGuild.prefix;
                }
                catch (Exception e)
                {
                    Terminal.WriteLine($"Error while loading guild ({saveGuild.id}): {e.Message}");
                }
            }

            #endregion
        }

        #region Bot data classes

        class BotData
        {
            public SaveUser[] users;
            public SaveStock[] stocks;
            public SaveGuild[] guilds;
        }

        class SaveUser
        {
            public SaveUser(string id, int money, DateTime lastDaily)
            {
                this.id = id;
                this.money = money;
                this.lastDaily = lastDaily;
            }

            public string id;
            public int money;
            public DateTime lastDaily;

            public SaveUserProperty[] properties;
            public SaveUserItem[] items;
            public SaveUserStock[] stocks;

            public class SaveUserProperty
            {
                public SaveUserProperty(string id, DateTime lastCollect)
                {
                    this.id = id;
                    this.lastCollect = lastCollect;
                }

                public string id;
                public DateTime lastCollect;
            }

            public class SaveUserItem
            {
                public SaveUserItem(string id, int quantity)
                {
                    this.id = id;
                    this.quantity = quantity;
                }

                public string id;
                public int quantity;
            }

            public class SaveUserStock
            {
                public SaveUserStock(string id, int quantity, int highBuyPrice)
                {
                    this.id = id;
                    this.quantity = quantity;
                    this.highBuyPrice = highBuyPrice;
                }

                public string id;
                public int quantity;
                public int highBuyPrice;
            }
        }

        class SaveStock
        {
            public SaveStock(string id, int price, bool wentUp, int[] lastPrices)
            {
                this.id = id;
                this.price = price;
                this.wentUp = wentUp;
                this.lastPrices = lastPrices;
            }

            public string id;
            public int price;
            public bool wentUp;
            public int[] lastPrices;
        }

        class SaveGuild
        {
            public SaveGuild(string id, string prefix)
            {
                this.id = id;
                this.prefix = prefix;
            }

            public string id;
            public string prefix;
        }

        #endregion

        #region Bot options

        public static BotOptions LoadBotOptions()
        {
            string path = Bot.DATA_PATH + "/config.json";
            string json = File.ReadAllText(path).Trim();

            BotOptions options = JsonConvert.DeserializeObject<BotOptions>(json);
            return options;
        }

        public class BotOptions
        {
            public string token;
            public string apiKey;

            public string prefix;
        }

        #endregion
    }
}
