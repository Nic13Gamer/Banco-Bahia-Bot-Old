using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace BancoBahiaBot.Modules
{
    public class FunCommands : ModuleBase<SocketCommandContext>
    {
        readonly Random random = new();

        [Command("Yes"), Alias("Sim")]
        [CommandHelp(
                name: "yes",
                uses: "yes",
                description: "Mande algum gif de sim, até aqueles mais estranhos"
            )]
        public async Task YesCommand()
        {
            string[] gifs =
            {
                "https://tenor.com/view/yes-hell-yes-dance-trump-funny-gif-13243504",
                "https://tenor.com/view/baby-scream-yeah-hockey-kid-angry-gif-13592395",
                "https://tenor.com/view/monkey-ape-dance-dancing-orangutan-gif-15714845",
                "https://tenor.com/view/baby-dancing-oh-yeah-baby-gif-16219181",
                "https://tenor.com/view/winnie-the-pooh-pooh-hungry-food-waiting-for-food-gif-17836848",
                "https://tenor.com/view/animal-muppets-yes-gif-14008611",
                "https://tenor.com/view/rq-yay-rq-yes-cat-rq-rq-yes-yes-yes-gif-20226294",
                "https://tenor.com/view/yes-agree-yup-nod-gif-14017499",
                "https://tenor.com/view/thumb-okay-ok-thumbs-up-yes-gif-15502990",
                "https://tenor.com/view/jana-janataffarel-taffarel-janaina-janainataffarel-gif-13335686",
                "https://tenor.com/view/yes-sir-spongebob-thumbs-up-gif-13785783",
                "https://tenor.com/view/yes-baby-yes-baby-b%C3%A9b%C3%A9-yes-b%C3%A9b%C3%A9-gif-21201958",
                "https://tenor.com/view/roblox-yes-sim-bow-gif-16416277",
                "https://tenor.com/view/he-man-yes-sir-hell-yeah-oh-yeah-gif-10397891",
                "https://tenor.com/view/pedro-approves-pedrorc-pedroredcerberus-yes-agree-gif-11599348",
                "https://tenor.com/view/yes-yes-yes-yes-yes-yes-yes-dog-yes-dog-dog-gif-21317058"
            };

            await Context.Channel.SendMessageAsync(gifs[random.Next(0, gifs.Length)]);
        }

    }
}
