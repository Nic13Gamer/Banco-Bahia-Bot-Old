using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace BancoBahiaBot.Modules
{
    public class PropertyCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Collect"), Alias("Coletar")]
        public async Task CollectCommand([Remainder]string property)
        {
            User user = UserHandler.GetUser(Context.User.Id.ToString());
            UserProperty userProperty = PropertyHandler.GetUserProperty(property, Context.User.Id.ToString());
            if(PropertyHandler.GetProperty(property) == null)
            {
                await Context.Channel.SendMessageAsync("Essa propriedade não existe!");
                return;
            }
            if (userProperty == null)
            {
                await Context.Channel.SendMessageAsync("Você não possui essa propriedade!");
                return;
            }

            string reply;
            DateTime lastCollect = userProperty.lastCollect;

            if(user.money < userProperty.property.tax)
            {
                await Context.Channel.SendMessageAsync("Você não tem dinheiro suficiente para pagar as taxas dessa propriedade!");
                return;
            }

            if (lastCollect.AddHours(12) <= DateTime.Now)
            {
                UserItem[] items = userProperty.property.items;
                string itemString = string.Empty;

                for (int i = 0; i < items.Length; i++)
                {
                    if (i != 0) itemString += ", ";
                    itemString += $"{items[i].quantity} de {items[i].item.name}";

                    ItemHandler.AddItemToUser(user, items[i]);
                }

                reply = $"Você ganhou `{itemString}` coletando essa {userProperty.property.name}! Gastando {userProperty.property.tax} de dinheiro em taxas de coleta.";

                Terminal.WriteLine($"Added | {itemString} | of {userProperty.property.id} to {Context.User} ({Context.User.Id})", Terminal.MessageType.INFO);
                user.money -= userProperty.property.tax;
                userProperty.lastCollect = DateTime.Now;
            }
            else
            {
                string remaining = (userProperty.lastCollect.AddHours(12) - DateTime.Now).ToString();
                remaining = remaining.Substring(0, remaining.LastIndexOf("."));
                reply = $"Sua {userProperty.property.name} ainda não está pronta! Tempo restante: {remaining}.";
            }

            await Context.Channel.SendMessageAsync(reply);
        }

        [Command("CollectAll"), Alias("ColetarTudo")]
        public async Task CollectAllCommand()
        {
            User user = UserHandler.GetUser(Context.User.Id.ToString());
            UserProperty[] userProperties = user.properties;

            if(userProperties.Length == 0)
            {
                await Context.Channel.SendMessageAsync("Você não tem propriedades!");
                return;
            }

            string itemsString = string.Empty;
            string propertiesString = string.Empty;
            int totalTax = 0;

            foreach (UserProperty userProperty in userProperties)
            {
                if (user.money < userProperty.property.tax) continue;

                if (propertiesString == string.Empty)
                    propertiesString += userProperty.property.name;
                else
                    propertiesString += ", " + userProperty.property.name;

                DateTime lastCollect = userProperty.lastCollect;

                if (lastCollect.AddHours(12) <= DateTime.Now)
                {
                    UserItem[] items = userProperty.property.items;
                    string itemString = string.Empty;

                    for (int i = 0; i < items.Length; i++)
                    {
                        if (i != 0) itemString += ", ";
                        itemString += $"{items[i].quantity} de {items[i].item.name}";

                        ItemHandler.AddItemToUser(user, items[i]);
                    }

                    if (itemsString == string.Empty)
                        itemsString += itemString;
                    else
                        itemsString += ", " + itemString;

                    Terminal.WriteLine($"Added | {itemString} | of {userProperty.property.id} to {Context.User} ({Context.User.Id})", Terminal.MessageType.INFO);
                    user.money -= userProperty.property.tax;
                    userProperty.lastCollect = DateTime.Now;

                    totalTax += userProperty.property.tax;
                }
            }

            if(totalTax == 0)
            {
                await Context.Channel.SendMessageAsync($"Você não tem propriedades para coletar!");
                return;
            }

            await Context.Channel.SendMessageAsync($"Você ganhou `{itemsString}` coletando sua `{propertiesString}`. Gastando {totalTax} de dinheiro em taxas de coleta!");
        }

        [Command("Buy"), Alias("Comprar")]
        public async Task BuyCommand([Remainder]string property)
        {
            User user = UserHandler.GetUser(Context.User.Id.ToString());

            UserProperty userProperty = PropertyHandler.GetUserProperty(property, Context.User.Id.ToString());
            if (PropertyHandler.GetProperty(property) == null)
            {
                await Context.Channel.SendMessageAsync("Essa propriedade não existe!");
                return;
            }
            if (userProperty != null)
            {
                await Context.Channel.SendMessageAsync("Você já possui essa propriedade!");
                return;
            }

            Property chosenProperty = PropertyHandler.GetProperty(property);

            if (user.money < chosenProperty.price)
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Mention}, você não tem dinheiro suficiente! Preço: {chosenProperty.price}");
                return;
            }

            user.money -= chosenProperty.price;

            UserProperty newProperty = new
                (
                    chosenProperty,
                    DateTime.Now.AddDays(-1)
                );

            PropertyHandler.AddUserProperty(user, newProperty);

            await Context.Channel.SendMessageAsync($"{chosenProperty.name} comprada com sucesso!");
            Terminal.WriteLine($"{Context.User} ({Context.User.Id}) bought {chosenProperty.id} succesfully!", Terminal.MessageType.INFO);
        }

        [Command("SellProperty"), Alias("VenderPropriedade")]
        public async Task SellPropertyCommand([Remainder] string property)
        {
            User user = UserHandler.GetUser(Context.User.Id.ToString());
            UserProperty userProperty = PropertyHandler.GetUserProperty(property, user.id);
            if (PropertyHandler.GetProperty(property) == null)
            {
                await Context.Channel.SendMessageAsync("Essa propriedade não existe!");
                return;
            }
            if (userProperty == null)
            {
                await Context.Channel.SendMessageAsync("Você não possui essa propriedade!");
                return;
            }

            var msg = await Context.Channel.SendMessageAsync($"Adicione a reação :white_check_mark: para vender sua {userProperty.property.name} por {userProperty.property.price / 3} de dinheiro! Você vai ganhar somente um terço do preço total.");

            ReactionHandler.AddReactionRequest(async (user, param) =>
            {
                await Context.Channel.SendMessageAsync($"debug reac {param}, por: " + user.Mention);

            }, new("✅"), msg, userProperty.property.id, true);
        }

        [Command("Properties"), Alias("Propriedades")]
        public async Task PropertiesCommand()
        {
            User user = UserHandler.GetUser(Context.User.Id.ToString());
            UserProperty[] userProperties = user.properties;
            string ownedProperties = string.Empty;
            string existingProperties = string.Empty;

            foreach (UserProperty userProperty in userProperties)
            {
                string remaining = (userProperty.lastCollect.AddHours(12) - DateTime.Now).ToString();
                remaining = remaining.Substring(0, remaining.LastIndexOf(".")) + " faltando";

                if (userProperty.lastCollect.AddHours(12) <= DateTime.Now)
                    remaining = "Coleta disponível";

                ownedProperties += $"`{userProperty.property.name} | {remaining}`\n";
            }

            foreach (Property property in PropertyHandler.properties)
            {
                existingProperties += $"`{property.name} | ${property.price}`\n";
            }

            EmbedBuilder embed = new EmbedBuilder
            {
                Title = "**PROPRIEDADES**",
                Color = Color.Orange
            }.WithAuthor(Context.User).WithCurrentTimestamp().WithFooter(footer => footer.Text = "Para ver mais sobre uma propriedade, use ?propriedade <nome>");

            if(ownedProperties != string.Empty)
                embed.AddField("> Propriedades possuídas",
                    ownedProperties);
            else
                embed.AddField("> Propriedades possuídas",
                    "`Você ainda não possui nenhuma propriedade`");

            embed.AddField("> Propriedades existentes",
                existingProperties);

            await Context.Channel.SendMessageAsync(Context.User.Mention, embed: embed.Build());
        }

        [Command("Properties"), Alias("Propriedades")]
        public async Task PropertiesCommand(IUser mention)
        {
            if (mention.IsBot) return;

            User user = UserHandler.GetUser(mention.Id.ToString());
            UserProperty[] userProperties = user.properties;
            string ownedProperties = string.Empty;
            string existingProperties = string.Empty;

            foreach (UserProperty userProperty in userProperties)
            {
                string remaining = (userProperty.lastCollect.AddHours(12) - DateTime.Now).ToString();
                remaining = remaining.Substring(0, remaining.LastIndexOf(".")) + " faltando";

                if (userProperty.lastCollect.AddHours(12) <= DateTime.Now)
                    remaining = "Coleta disponível";

                ownedProperties += $"`{userProperty.property.name} | {remaining}`\n";
            }

            foreach (Property property in PropertyHandler.properties)
            {
                existingProperties += $"`{property.name} | ${property.price}`\n";
            }

            EmbedBuilder embed = new EmbedBuilder
            {
                Title = "**PROPRIEDADES**",
                Color = Color.Orange
            }.WithAuthor(mention).WithCurrentTimestamp().WithFooter(footer => footer.Text = "Para ver mais sobre uma propriedade, use ?propriedade <nome>");

            if (ownedProperties != string.Empty)
                embed.AddField("> Propriedades possuídas",
                    ownedProperties);
            else
                embed.AddField("> Propriedades possuídas",
                    $"`{mention.Username} ainda não possui nenhuma propriedade`");

            embed.AddField("> Propriedades existentes",
                existingProperties);

            await Context.Channel.SendMessageAsync(Context.User.Mention, embed: embed.Build());
        }

        [Command("Property"), Alias("Propriedade")]
        public async Task PropertyCommand([Remainder]string property)
        {
            User user = UserHandler.GetUser(Context.User.Id.ToString());
            Property chosenProperty = PropertyHandler.GetProperty(property);
            if(chosenProperty == null)
            {
                await Context.Channel.SendMessageAsync("Essa propriedade não existe!");
                return;
            }

            string ownedEmoji;
            UserProperty userProperty = PropertyHandler.GetUserProperty(property, Context.User.Id.ToString());
            if (userProperty == null)
                ownedEmoji = ":x:";
            else
                ownedEmoji = ":white_check_mark:";

            EmbedBuilder embed = new EmbedBuilder
            {
                Title = $"**{chosenProperty.name}** | {ownedEmoji}",
                Color = Color.Orange
            }.WithAuthor(Context.User).WithCurrentTimestamp().WithFooter(footer => footer.Text = $"Informações de {chosenProperty.name}");

            embed.AddField("> **Preço**",
                $"`${chosenProperty.price}`");

            string itemString = string.Empty;
            for (int i = 0; i < chosenProperty.items.Length; i++)
            {
                if (i != 0) itemString += "\n";
                itemString += $"{chosenProperty.items[i].quantity} de {chosenProperty.items[i].item.name}";
            }

            embed.AddField("> **Itens por coleta**",
                $"`{itemString}`");

            embed.AddField("> Informações",
                $"`Taxas por coleta: ${chosenProperty.tax}`");

            embed.AddField("> Descrição",
                $"`{chosenProperty.description}`");

            await Context.Channel.SendMessageAsync(Context.User.Mention, embed: embed.Build());
        }

        struct SellPropertyRequest
        {
            public SellPropertyRequest(IUser user, UserProperty property)
            {
                this.user = user;
                this.property = property;
            }

            public IUser user;
            public UserProperty property;
        }
    }
}