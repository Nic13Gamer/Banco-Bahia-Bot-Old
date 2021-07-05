using Discord.WebSocket;
using SimpleJSON;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace BancoBahiaBot
{
    class UserHandler
    {
        public static DiscordSocketClient client;

        static readonly List<User> users = new List<User>();

        public static void LoadUsersData()
        {
            string path = "C:/Users/nicho/Desktop/BancoBahia/botData.json";
            if (!File.Exists(path)) File.Create(path);
            string rawJson = File.ReadAllText(path); if (rawJson.Trim() == string.Empty) { SaveUsersData(); rawJson = File.ReadAllText(path); }
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

                List<UserProperty> properties = new List<UserProperty>();

                foreach (JSONObject userPropertyJson in user["properties"])
                {
                    Property property = null;

                    foreach (Property _property in PropertyHandler.properties)
                    {
                        if (_property.id == userPropertyJson["id"])
                        {
                            property = _property;
                            break;
                        }
                    }

                    UserProperty userProperty = new UserProperty
                        (
                            property,
                            DateTime.Parse(userPropertyJson["lastCollect"])
                        );

                    properties.Add(userProperty);
                }

                newUser.properties = properties.ToArray();

                #endregion
            }
            catch (Exception e)
            {
                Terminal.WriteLine($"Error while loading user data: {e.Message}", Terminal.MessageType.ERROR);
                return;
            }
            
        }

        public static void SaveUsersData()
        {
            string path = "C:/Users/nicho/Desktop/BancoBahia/botData.json";

            JSONObject json = new JSONObject();
            JSONObject usersJson = new JSONObject();

            foreach (User user in users)
            {
                JSONObject userArray = new JSONObject();

                userArray.Add("id", user.id);
                userArray.Add("money", user.money);
                userArray.Add("lastDaily", user.lastDaily.ToString());

                #region Save Properties

                JSONObject userProperties = new JSONObject();

                foreach (UserProperty property in user.properties)
                {
                    JSONObject userProperty = new JSONObject();

                    userProperty.Add("id", property.property.id);
                    userProperty.Add("lastCollect", property.lastCollect.ToString());

                    userProperties.Add(property.property.id, userProperty);
                }

                userArray.Add("properties", userProperties);

                #endregion

                usersJson.Add(user.id, userArray);
            }

            json.Add("users", usersJson);

            try
            {
                File.WriteAllText(path, json.ToString());
            }
            catch (Exception e)
            {
                Terminal.WriteLine(e.Message, Terminal.MessageType.ERROR);
            }
        }

        public static User GetUser(string id)
        {
            User user = CreateUser(id);
            return user;
        }

        public static User CreateUser(string id)
        {
            foreach (User _user in users)
                if (_user.id == id) return _user;

            User user = new User
                (
                    id: id,
                    money: 0,
                    lastDaily: DateTime.Now.AddDays(-1),
                    properties: new UserProperty[] { }
                );

            users.Add(user);
            return user;
        }

        public static void AddUserProperty(User user, UserProperty property)
        {
            List<UserProperty> userProperties = user.properties.ToList();

            userProperties.Add(property);

            user.properties = userProperties.ToArray();
        }
    }

    class User
    {
        public User(string id, int money, DateTime lastDaily, UserProperty[] properties)
        {
            this.id = id;
            this.money = money;
            this.lastDaily = lastDaily;
            this.properties = properties;
        }

        public string id;
        public int money;
        public DateTime lastDaily;

        public UserProperty[] properties;
    }
}
