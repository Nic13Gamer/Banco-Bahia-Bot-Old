using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;

namespace BancoBahiaBot
{
    class UserHandler
    {
        static readonly string botDataPath = Bot.DATA_PATH + "/botData.json";

        static readonly List<User> users = new();

        public static void LoadUsersData()
        {
            if (!File.Exists(botDataPath)) File.Create(botDataPath);
            string rawJson = File.ReadAllText(botDataPath); if (rawJson.Trim() == string.Empty) { SaveManager.SaveAll(); rawJson = File.ReadAllText(botDataPath); }
            JSONObject json = (JSONObject)JSON.Parse(rawJson);

            foreach (JSONObject user in json["users"])
                LoadUserFromJson(user);
        }

        static void LoadUserFromJson(JSONObject user)
        {
            try
            {
                User newUser = CreateUser(user["id"]);

                newUser.money = user["money"];
                newUser.lastDaily = DateTime.Parse(user["lastDaily"]);

                #region Load Properties

                List<UserProperty> properties = new();

                foreach (JSONObject userPropertyJson in user["properties"])
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

                #region Load Inventory

                List<UserItem> inventory = new();

                foreach (JSONObject userItemJson in user["inventory"])
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
            }
            catch (Exception e)
            {
                Terminal.WriteLine($"Error while loading user data: {e.Message} | User: {(string)user["id"]}", Terminal.MessageType.ERROR);
                return;
            }

        }

        public static User GetUser(string id) => CreateUser(id);

        public static User CreateUser(string id)
        {
            foreach (User _user in users)
                if (_user.id == id) return _user;

            User user = new
                (
                    id: id,
                    money: 0,
                    lastDaily: DateTime.Now.AddDays(-1),
                    properties: Array.Empty<UserProperty>(),
                    inventory: Array.Empty<UserItem>(),
                    stocks: Array.Empty<UserStock>()
                );

            users.Add(user);
            return user;
        }

        public static User[] GetUsers() => users.ToArray();
    }

    class User
    {
        public User(string id, int money, DateTime lastDaily, UserProperty[] properties, UserItem[] inventory, UserStock[] stocks)
        {
            this.id = id;
            this.money = money;
            this.lastDaily = lastDaily;
            this.properties = properties;
            this.inventory = inventory;
            this.stocks = stocks;
        }

        public string id;
        public int money;
        public DateTime lastDaily;

        public UserProperty[] properties;
        public UserItem[] inventory;
        public UserStock[] stocks;
    }
}
