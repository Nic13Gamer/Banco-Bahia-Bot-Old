﻿using BancoBahiaBot.Utils;

using System.Collections.Generic;
using System.Linq;

namespace BancoBahiaBot
{
    class ItemHandler
    {
        public static readonly List<Item> items = new();

        #region Define Items

        public static readonly Item bread = new
            (
                id: "bread",
                name: "Pão",
                description: "Macio igual a um bolo",
                sellPrice: 60
            );

        public static readonly Item clothes = new
            (
                id: "clothes",
                name: "Roupa",
                description: "Feita de tecido caro",
                sellPrice: 100
            );

        public static readonly Item screen = new
            (
                id: "screen",
                name: "Tela",
                description: "Tela de alta resolução",
                sellPrice: 340
            );

        #endregion

        public static void Start()
        {
            items.Add(bread);
            items.Add(clothes);
            items.Add(screen);
        }

        public static Item GetItem(string item)
        {
            item = StringUtils.RemoveAccents(item.ToLower());

            foreach (Item _item in items)
            {
                if (StringUtils.RemoveAccents(_item.id.ToLower()) == item || StringUtils.RemoveAccents(_item.name.ToLower()) == item)
                {
                    return _item;
                }
            }

            return null;
        }

        public static UserItem GetUserItem(Item item, User user)
        {
            UserItem userItem = null;
            foreach (UserItem _userItem in user.items)
            {
                if(_userItem.item == item)
                {
                    userItem = _userItem;
                    break;
                }
            }

            return userItem;
        }

        public static void AddItemToUser(User user, Item item, int quantity = 1)
        {
            List<UserItem> userInventory = user.items.ToList();

            foreach (UserItem userItem in userInventory)
            {
                if(userItem.item == item)
                {
                    userItem.quantity += quantity;
                    user.items = userInventory.ToArray();

                    return;
                }
            }

            UserItem newUserItem = new(item, quantity);

            userInventory.Add(newUserItem);

            user.items = userInventory.ToArray();
        }

        public static void RemoveItemFromUser(User user, Item item, int quantity = 1)
        {
            List<UserItem> userInventory = user.items.ToList();

            foreach (UserItem userItem in user.items)
            {
                if (userItem.item == item)
                {
                    userItem.quantity -= quantity;

                    if(userItem.quantity <= 0)
                    {
                        userInventory.Remove(userItem);
                    }
                }
            }

            user.items = userInventory.ToArray();
        }
    }

    public class Item
    {
        public Item(string id, string name, string description, int sellPrice)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.sellPrice = sellPrice;
        }

        public string id;
        public string name;
        public string description;
        public int sellPrice;
    }

    public class UserItem
    {
        public UserItem(Item item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
        }

        public Item item;
        public int quantity;
    }
}
