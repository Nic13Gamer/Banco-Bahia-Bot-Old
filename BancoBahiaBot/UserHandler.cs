using Discord.WebSocket;
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
            string rawJson = File.ReadAllText(botDataPath); if (rawJson.Trim() == string.Empty) { SaveUsersData(); rawJson = File.ReadAllText(botDataPath); }
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

        public static void SaveUsersData()
        {
            JSONObject json = new();
            JSONObject usersJson = new();

            foreach (User user in users)
            {
                JSONObject userArray = new();

                userArray.Add("id", user.id);
                userArray.Add("money", user.money);
                userArray.Add("lastDaily", user.lastDaily.ToString());

                #region Save Properties

                JSONObject userProperties = new();

                foreach (UserProperty property in user.properties)
                {
                    JSONObject userProperty = new();

                    userProperty.Add("id", property.property.id);
                    userProperty.Add("lastCollect", property.lastCollect.ToString());

                    userProperties.Add(property.property.id, userProperty);
                }

                userArray.Add("properties", userProperties);

                #endregion

                #region Save Inventory

                JSONObject userInventory = new();

                foreach (UserItem item in user.inventory)
                {
                    JSONObject userItem = new();

                    userItem.Add("id", item.item.id);
                    userItem.Add("quantity", item.quantity);

                    userInventory.Add(item.item.id, userItem);
                }

                userArray.Add("inventory", userInventory);

                #endregion

                usersJson.Add(user.id, userArray);
            }

            json.Add("users", usersJson);

            try
            {
                File.WriteAllText(botDataPath, json.ToString());
            }
            catch (Exception e)
            {
                Terminal.WriteLine(e.Message, Terminal.MessageType.ERROR);
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
                    inventory: Array.Empty<UserItem>()
                );

            users.Add(user);
            return user;
        }
    }

    class User
    {
        public User(string id, int money, DateTime lastDaily, UserProperty[] properties, UserItem[] inventory)
        {
            this.id = id;
            this.money = money;
            this.lastDaily = lastDaily;
            this.properties = properties;
            this.inventory = inventory;
        }

        public string id;
        public int money;
        public DateTime lastDaily;

        public UserProperty[] properties;
        public UserItem[] inventory;
    }
}
