using System;
using Discord;
using System.Collections.Generic;

namespace BancoBahiaBot
{
    class HelpHandler
    {
        static readonly List<CommandHelp> commandHelps = new();

        public static void AddCommandHelp(CommandHelp commandHelp)
        {
            foreach (CommandHelp command in commandHelps)
            {
                if (commandHelp.name.ToLower() == command.name.ToLower() || commandHelp.alias.ToLower() == command.alias.ToLower())
                    return;
            }

            commandHelps.Add(commandHelp);
        }

        public static Embed GetCommandHelpEmbed(string name, Guild guild)
        {
            CommandHelp commandHelp = GetCommandHelp(name);
            if (commandHelp == null) return null;

            EmbedBuilder embed = new EmbedBuilder
            {
                Title = "**AJUDA**",
                Color = Color.Orange
            }.WithCurrentTimestamp().WithFooter(footer => footer.Text = $"Ajuda do comando {commandHelp.name}");

            embed.AddField("> Uso do comando",
                    $"`{guild.prefix}{commandHelp.use}`");

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

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    class CommandHelpAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="alias"></param>
        /// <param name="use">No need to put a prefix in the start of string, it is automatic</param>
        /// <param name="description"></param>
        /// <param name="permissions"></param>
        public CommandHelpAttribute(string name, string alias, string use, string description, GuildPermission[] permissions) =>
            HelpHandler.AddCommandHelp(new(name, alias, use, description, permissions));
    }
}
