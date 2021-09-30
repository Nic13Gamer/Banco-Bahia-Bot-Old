using System;
using System.Collections.Generic;

namespace BancoBahiaBot
{
    class UserHandler
    {
        static readonly List<User> users = new();

        public static User GetUser(ulong id) => CreateUser(id);

        public static User CreateUser(ulong id)
        {
            foreach (User _user in users)
                if (_user.id == id.ToString()) return _user;

            User user = new
                (
                    id: id.ToString(),
                    money: 0,
                    lastDaily: DateTime.Now.AddDays(-1),
                    properties: Array.Empty<UserProperty>(),
                    items: Array.Empty<UserItem>(),
                    stocks: Array.Empty<UserStock>()
                );

            users.Add(user);
            return user;
        }

        public static User[] GetUsers() => users.ToArray();
    }

    class User
    {
        public User(string id, int money, DateTime lastDaily, UserProperty[] properties, UserItem[] items, UserStock[] stocks)
        {
            this.id = id;
            this.money = money;
            this.lastDaily = lastDaily;
            this.properties = properties;
            this.items = items;
            this.stocks = stocks;
        }

        public string id;
        public int money;
        public DateTime lastDaily;

        public UserProperty[] properties;
        public UserItem[] items;
        public UserStock[] stocks;
    }
}
