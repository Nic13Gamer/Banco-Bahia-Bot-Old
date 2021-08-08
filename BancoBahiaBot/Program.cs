namespace BancoBahiaBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new();
            bot.StartBot().GetAwaiter().GetResult();
        }
    }
}
