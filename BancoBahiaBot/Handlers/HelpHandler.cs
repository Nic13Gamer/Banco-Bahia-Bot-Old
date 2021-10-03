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

            string commandUses = string.Empty;
            foreach (var use in commandHelp.uses.Split("||"))
            {
                if (commandUses == string.Empty)
                    commandUses += $"`{guild.prefix}{use}`";
                else
                    commandUses += $"\n`{use}`";

            }
            commandUses = commandUses.Replace("{prefix}", guild.prefix);

            embed.AddField("> Usos do comando",
                    commandUses);

            embed.AddField("> Descrição",
                    commandHelp.description);

            if (commandHelp.permissions != null)
            {
                string permissionsString = string.Empty;
                for (int i = 0; i < commandHelp.permissions.Length; i++)
                {
                    if (i != 0) permissionsString += ", ";

                    permissionsString += commandHelp.permissions[i];
                }

                embed.AddField("> Permissões necessárias",
                    permissionsString);
            }
            else
                embed.AddField("> Permissões necessárias",
                    "Nenhuma");

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
        public CommandHelp(string name, string alias, string uses, string description, GuildPermission[] permissions)
        {
            this.name = name;
            this.alias = alias;
            this.uses = uses;
            this.description = description;
            this.permissions = permissions;
        }

        public string name;
        public string alias;
        public string uses;
        public string description;
        public GuildPermission[] permissions;
    }

    /// <summary>
    /// Add this attribute at the commands that you want to show help to the user.
    /// If you have more than one method for a command add this attribute just once.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    class CommandHelpAttribute : Attribute
    {
        /// <summary>
        /// Add this attribute at the commands that you want to show help to the user.
        /// If you have more than one method for a command add this attribute just once.
        /// </summary>
        /// <param name="name">Name of command.</param>
        /// <param name="alias">Alias of command.</param>
        /// <param name="uses">Use of the command. No need to put a prefix in the start of string, it is automatic.
        /// If you need the prefix again, just insert {prefix} .
        /// Use || to separate lines.</param>
        /// <param name="description">Description of the command.</param>
        /// <param name="permissions">Permissions needed to run the command.</param>
        public CommandHelpAttribute(string name, string alias, string uses, string description, GuildPermission[] permissions = null) =>
            HelpHandler.AddCommandHelp(new(name, alias, uses, description, permissions));
    }
}
