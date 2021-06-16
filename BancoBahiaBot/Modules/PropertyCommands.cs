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
            UserProperty userProperty = GetUserProperty(property);
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

            if (lastCollect.AddHours(12) <= DateTime.Now)
            {
                int money = userProperty.property.dailyMoney - userProperty.property.dailyTax;
                reply = $"Você ganhou {userProperty.property.dailyMoney} de dinheiro coletando essa {userProperty.property.name}! E gastou {userProperty.property.dailyTax} de dinheiro em taxas de coleta.";

                Terminal.WriteLine($"Added {money} of {userProperty.property.id} money to {Context.User} ({Context.User.Id})", Terminal.MessageType.INFO);
                user.money += money;
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

        [Command("Buy"), Alias("Comprar")]
        public async Task BuyCommand([Remainder]string property)
        {
            User user = UserHandler.GetUser(Context.User.Id.ToString());

            UserProperty userProperty = GetUserProperty(property);
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
            if(chosenProperty == null)
                return;

            if (user.money < chosenProperty.price)
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Mention}, você não tem dinheiro suficiente! Preço: {chosenProperty.price}");
                return;
            }

            user.money -= chosenProperty.price;

            UserProperty newProperty = new UserProperty
                (
                    chosenProperty,
                    DateTime.Now.AddDays(-1)
                );

            UserHandler.AddUserProperty(user, newProperty);

            await Context.Channel.SendMessageAsync($"{chosenProperty.name} comprada com sucesso!");
            Terminal.WriteLine($"{Context.User} ({Context.User.Id}) bought {chosenProperty.id} succesfully!", Terminal.MessageType.INFO);
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

            await Context.Channel.SendMessageAsync(embed: embed.Build());
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

            await Context.Channel.SendMessageAsync(embed: embed.Build());
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
            UserProperty userProperty = GetUserProperty(property);
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

            embed.AddField("> **Descrição**",
                $"`{chosenProperty.description}`");

            embed.AddField("> Informações",
                $"`Dinheiro por coleta: ${chosenProperty.dailyMoney}`\n" +
                $"`Taxa por coleta: ${chosenProperty.dailyTax}`");

            embed.AddField("> Requer",
                $"`TODO`"); // TODO

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }

        UserProperty GetUserProperty(string property)
        {
            User user = UserHandler.GetUser(Context.User.Id.ToString());
            Property chosenProperty = PropertyHandler.GetProperty(property);

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
    }
}