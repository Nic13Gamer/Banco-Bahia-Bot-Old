using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace BancoBahiaBot.Modules
{
    public class DebugCommands : ModuleBase<SocketCommandContext>
    {
        readonly Random random = new Random();

        [Command("Pergunta"), Alias("Question")]
        public async Task QuestionCommand([Remainder]string args)
        {
            bool chance = Convert.ToBoolean(random.Next(0, 2));
            string reply = $"{args} : {chance}\n\nPergunta feita por: {Context.User.Mention}!";

            await Context.Channel.SendMessageAsync(reply);
            Terminal.WriteLine($"Replying to {Context.User} ({Context.User.Id}):\n{reply}", Terminal.MessageType.INFO, ConsoleColor.Gray);
        }

        [Command("SaveLog")]
        public Task SaveLogCommand()
        {
            string path = "C:/Users/nicho/Desktop/BancoBahia/log.txt";

            Terminal.OutputLog(path);

            return null;
        }
    }
}
