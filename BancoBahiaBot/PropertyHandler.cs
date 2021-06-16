using System;
using System.Collections.Generic;

namespace BancoBahiaBot
{
    class PropertyHandler
    {
        public static readonly List<Property> properties = new List<Property>();

        public static readonly Property bakery = new Property
            (
                id: "bakery",
                name: "Padaria",
                description: "Uma padaria que sai pão todo dia.",
                dailyMoney: 2000,
                dailyTax: 350,
                price: 10000
            );

        public static readonly Property clothingStore = new Property
            (
                id: "clothingStore",
                name: "Loja de roupas",
                description: "Uma variedade bem grande de roupas.",
                dailyMoney: 5000,
                dailyTax: 750,
                price: 25000
            );

        public static readonly Property electronicStore = new Property
            (
                id: "electronicStore",
                name: "Loja de eletronicos",
                description: "Vende eletrônicos de ultima geração.",
                dailyMoney: 8000,
                dailyTax: 1050,
                price: 35000
            );

        public static void Start()
        {
            properties.Add(bakery);
            properties.Add(clothingStore);
            properties.Add(electronicStore);
        }

        public static Property GetProperty(string property)
        {
            property = property.ToLower();

            foreach (Property _property in properties)
            {
                if (_property.id.ToLower() == property || _property.name.ToLower() == property)
                {
                    return _property;
                }
            }

            return null;
        }
    }

    public class Property
    {
        public Property(string id, string name, string description, int dailyMoney, int dailyTax, int price)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.dailyMoney = dailyMoney;
            this.dailyTax = dailyTax;
            this.price = price;
        }

        public string id;

        public string name;
        public string description;
        public int dailyMoney;
        public int dailyTax;
        public int price;
    }

    public class UserProperty
    {
        public UserProperty(Property property, DateTime lastCollect)
        {
            this.property = property;
            this.lastCollect = lastCollect;
        }

        public Property property;
        public DateTime lastCollect;
    }
}
