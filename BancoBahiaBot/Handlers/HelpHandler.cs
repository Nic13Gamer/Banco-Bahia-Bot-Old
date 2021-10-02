using Discord;
using System.Collections.Generic;

namespace BancoBahiaBot
{
    class HelpHandler
    {
        #region Command help instances

        static readonly CommandHelp prefixCommandHelp = new
                (
                    name: "prefix",
                    alias: "prefixo",
                    use: "prefix <novo prefixo>",
                    description: "Muda o prefixo de comandos do servidor",
                    permissions: new[] { GuildPermission.ManageGuild }
                );

        #endregion

        static readonly List<CommandHelp> commandHelps = new();

        public static void Start()
        {
            AddCommandHelp(prefixCommandHelp);
        }

        public static void AddCommandHelp(CommandHelp commandHelp)
        {
            foreach (CommandHelp command in commandHelps)
            {
                if (commandHelp.name.ToLower() == command.name.ToLower() || commandHelp.alias.ToLower() == command.alias.ToLower())
                    return;
            }

            commandHelps.Add(commandHelp);
        }

        public static Embed GetCommandHelpEmbed(string name)
        {
            CommandHelp commandHelp = GetCommandHelp(name);
            if (commandHelp == null) return null;

            EmbedBuilder embed = new EmbedBuilder
            {
                Title = "**AJUDA**",
                Color = Color.Orange
            }.WithCurrentTimestamp().WithFooter(footer => footer.Text = $"Ajuda do comando {commandHelp.name}");

            embed.AddField("> Uso do comando",
                    $"`{commandHelp.use}`");

            embed.AddField("> Descrição",
                    commandHelp.description);

            string permissionsString = string.Empty;
            for (int i = 0; i < commandHelp.permissions.Length; i++)
            {
                if (i != 0) permissionsString += ", ";

                permissionsString += commandHelp.permissions[i];
            }

            embed.AddField("> Permissoes necessárias",
                    permissionsString);

            return embed.Build();
        }

        public static CommandHelp GetCommandHelp(string name)
        {
            foreach (CommandHelp command in commandHelps)
            {
                if (name.ToLower() == command.name.ToLower() || name.ToLower() == command.alias.ToLower())
                    return command;
            }

            return null;
        }
    }

    class CommandHelp
    {
        public CommandHelp(string name, string alias, string use, string description, GuildPermission[] permissions)
        {
            this.name = name;
            this.alias = alias;
            this.use = use;
            this.description = description;
            this.permissions = permissions;
        }

        public string name;
        public string alias;
        public string use;
        public string description;
        public GuildPermission[] permissions;
    }
}
