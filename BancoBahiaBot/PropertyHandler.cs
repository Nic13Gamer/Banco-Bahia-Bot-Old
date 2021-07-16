using System;
using System.Collections.Generic;
using System.Linq;

namespace BancoBahiaBot
{
    class PropertyHandler
    {
        public static readonly List<Property> properties = new List<Property>();

        #region Define properties

        #region Bakery

        static readonly UserItem[] bakeryUserItems = new UserItem[]
        {
            new UserItem
                (
                    item: ItemHandler.GetItem("bread"),
                    quantity: 35
                )
        };

        public static readonly Property bakery = new Property
            (
                id: "bakery",
                name: "Padaria",
                description: "Uma padaria que sai pão todo dia.",
                items: bakeryUserItems,
                tax: 350,
                price: 10000
            );

        #endregion

        #region Clothing Factory

        static readonly UserItem[] clothingFactoryUserItems = new UserItem[]
        {
            new UserItem
                (
                    item: ItemHandler.GetItem("clothes"),
                    quantity: 50
                )
        };

        public static readonly Property clothingFactory = new Property
            (
                id: "clothingFactory",
                name: "Fábrica de roupas",
                description: "Fabrica uma variedade bem grande de roupas.",
                items: clothingFactoryUserItems,
                tax: 750,
                price: 25000
            );

        #endregion

        #region Screen Factory

        static readonly UserItem[] screenFactoryUserItems = new UserItem[]
        {
            new UserItem
                (
                    item: ItemHandler.GetItem("screen"),
                    quantity: 25
                )
        };

        public static readonly Property screenFactory = new Property
            (
                id: "screenFactory",
                name: "Fábrica de telas",
                description: "Fabrica telas de alta resolução.",
                items: screenFactoryUserItems,
                tax: 1050,
                price: 40000
            );

        #endregion

        #endregion

        public static void Start()
        {
            properties.Add(bakery);
            properties.Add(clothingFactory);
            properties.Add(screenFactory);
        }

        public static Property GetProperty(string property)
        {
            property = RemoveAccents(property.ToLower());

            foreach (Property _property in properties)
            {
                if (RemoveAccents(_property.id.ToLower()) == property|| RemoveAccents(_property.name.ToLower()) == property)
                {
                    return _property;
                }
            }

            return null;
        }

        public static UserProperty GetUserProperty(string property, string id)
        {
            User user = UserHandler.GetUser(id);
            Property chosenProperty = GetProperty(property);

            if (chosenProperty == null)
                return null;

            UserProperty userProperty = null;
            foreach (UserProperty _userProperty in user.properties)
            {
                if (_userProperty.property == chosenProperty)
                {
                    userProperty = _userProperty;
                    break;
                }
            }

            return userProperty;
        }

        public static void AddUserProperty(User user, UserProperty property)
        {
            List<UserProperty> userProperties = user.properties.ToList();

            userProperties.Add(property);

            user.properties = userProperties.ToArray();
        }

        static string RemoveAccents(string text)
        {
            var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }
    }

    public class Property
    {
        public Property(string id, string name, string description, UserItem[] items, int tax, int price)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.tax = tax;
            this.items = items;
            this.price = price;
        }

        public string id;

        public string name;
        public string description;
        public int tax;
        public UserItem[] items;
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
