using System;
using System.Collections.Generic;
using System.IO;

namespace BancoBahiaBot
{
    class Terminal
    {
        static readonly List<string> consoleLog = new();

        public static void WriteLine(string msg, MessageType type = MessageType.INFO, ConsoleColor color = ConsoleColor.White)
        {
            AddToLog(msg, type);

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
            ERROR
        }
    }
}
