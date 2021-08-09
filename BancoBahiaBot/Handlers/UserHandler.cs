using System;
using System.Collections.Generic;

namespace BancoBahiaBot
{
    class UserHandler
    {
        static readonly List<User> users = new();

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
