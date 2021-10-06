using BancoBahiaBot.Utils;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BancoBahiaBot
{
    class Terminal
    {
        static readonly List<string> consoleLog = new();

        public static void Start()
        {
            Thread thread = new(async () =>
            {
                while (true)
                {
                    try
                    {
                        string[] args = Console.ReadLine().Split(" ");

                        switch (args[0])
                        {
                            case "set_game":
                                {
                                    string text = StringUtils.GetAllRemainderTextAfter(args, 0);

                                    await Bot.Client.SetGameAsync(text);

                                    WriteLine("Set bot game to: " + text, MessageType.CMD);

                                    break;
                                }

                            case "guilds_notify":
                                {
                                    string text = StringUtils.GetAllRemainderTextAfter(args, 0);

                                    foreach (var guild in Bot.Client.Guilds)
                                        await guild.DefaultChannel.SendMessageAsync(text);

                                    WriteLine($"Notified {Bot.Client.Guilds.Count} guilds with: {text}", MessageType.CMD);

                                    break;
                                }

                            case "guild_channel_send":
                                {
                                    string channelId = args[1];
                                    string text = StringUtils.GetAllRemainderTextAfter(args, 1);
                                    var channel = Bot.Client.GetChannel(ulong.Parse(channelId));

                                    await (channel as SocketTextChannel).SendMessageAsync(text);

                                    WriteLine($"Send to channel id ({channelId}): {text}", MessageType.CMD);

                                    break;
                                }
                            }
                        }
                    catch (Exception e)
                    {
                        WriteLine(e.Message, MessageType.CMD);
                    }
                }
            });

            thread.Start();
        }

        public static void WriteLine(object msg, MessageType type = MessageType.INFO, ConsoleColor color = ConsoleColor.White)
        {
            AddToLog(msg.ToString(), type);

            if (type == MessageType.INFO)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write($"[{type}] ");
            } else if (type == MessageType.WARN)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"[{type}] ");
            } else if (type == MessageType.ERROR)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"[{type}] ");
            } else if (type == MessageType.CMD)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write($"[{type}] ");
            }

            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void AddToLog(string msg, MessageType type)
        {
            consoleLog.Add($"[{type}] {msg}\n");
        }

        public static void SaveLog()
        {
            string log = string.Empty;

            foreach(string line in consoleLog)
                log += line;

            try
            {
                File.WriteAllText(Bot.DATA_PATH + "/log.txt", log);
                WriteLine($"Saved log to {Bot.DATA_PATH + "/log.txt"}!", Terminal.MessageType.INFO);
            }
            catch (Exception e)
            {
                WriteLine(e.Message, MessageType.ERROR);
            }
        }

        public enum MessageType
        {
            INFO,
            WARN,
            ERROR,
            CMD
        }
    }
}
