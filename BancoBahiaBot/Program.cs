namespace BancoBahiaBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            bot.StartBot().GetAwaiter().GetResult();
        }
    }
}
