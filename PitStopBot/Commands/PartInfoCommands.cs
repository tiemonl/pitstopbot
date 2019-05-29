using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PitStopBot.Objects;
using PitStopBot.Utils;

namespace PitStopBot.Commands {
    public class PartInfoCommands : ModuleBase {

        private PartInfoUtils partUtils = new PartInfoUtils();
        private EmbedBuilder MyEmbedBuilder = new EmbedBuilder();
        private readonly Color legendaryColor = new Color(196, 181, 59);
        private readonly Color epicColor = new Color(133, 212, 225);
        private readonly Color rareColor = new Color(218, 146, 65);
        private readonly Color commonColor = new Color(168, 161, 174);

        [Command("part"), Summary("returns the parts count")]
        public async Task GetPartInfo([Summary("Part NFT #")] string num) {
            Part part = await partUtils.GetPart(num);
            Details detail = part.details;
            Color color = detail.rarity.Equals("Legendary") ? legendaryColor :
                detail.rarity.Equals("Epic") ? epicColor :
                detail.rarity.Equals("Rare") ? rareColor : commonColor;

            MyEmbedBuilder.WithTitle("Part Info");
            MyEmbedBuilder.WithColor(color);
            MyEmbedBuilder.WithImageUrl(part.image);
            MyEmbedBuilder.AddField("Brand", detail.brand, true);
            MyEmbedBuilder.AddField("Name", part.name, true);
            MyEmbedBuilder.AddField("Model", detail.model, true);
            MyEmbedBuilder.AddField("Type", detail.type, true);
            MyEmbedBuilder.AddField("Rarity", detail.rarity, true);
            MyEmbedBuilder.AddField("Is Elite?", detail.isElite, true);
            MyEmbedBuilder.AddField("Serial Number", detail.serialNumber, true);
            MyEmbedBuilder.AddField("Durability", detail.durability, true);
            MyEmbedBuilder.AddField("Weight", detail.weight, true);
            MyEmbedBuilder.AddField("Steering", detail.steering, true);
            MyEmbedBuilder.AddField("Power", detail.power, true);
            MyEmbedBuilder.AddField("Speed", detail.speed, true);

            await ReplyAsync(embed: MyEmbedBuilder.Build());
        }

        [Command("compare"), Summary("returns the parts count")]
        public async Task GetPartInfo([Summary("Part NFT #")] string num, string num2) {
            Part part = await partUtils.GetPart(num);
            Part part2 = await partUtils.GetPart(num2);
            Details detail = part.details;
            Details detail2 = part2.details;
            Color color = detail.rarity.Equals("Legendary") ? legendaryColor :
                detail.rarity.Equals("Epic") ? epicColor :
                detail.rarity.Equals("Rare") ? rareColor : commonColor;

            MyEmbedBuilder.WithTitle("Part Comparison");
            MyEmbedBuilder.WithColor(color);
            MyEmbedBuilder.AddField("Brand", $"{detail.brand}\n{detail2.brand}", true);
            MyEmbedBuilder.AddField("Name", $"{part.name}\n{part2.name}", true);
            MyEmbedBuilder.AddField("Model", $"{detail.model}\n{detail2.model}", true);
            MyEmbedBuilder.AddField("Type", $"{detail.type}\n{detail2.type}", true);
            MyEmbedBuilder.AddField("Rarity", $"{detail.rarity}\n{detail2.rarity}", true);
            MyEmbedBuilder.AddField("Is Elite?", $"{detail.isElite}\n{detail2.isElite}", true);
            MyEmbedBuilder.AddField("Serial Number", ComparisonFormatterLesserThan(int.Parse(detail.serialNumber), int.Parse(detail2.serialNumber)), true);
            MyEmbedBuilder.AddField("Durability", ComparisonFormatterGreaterThan(detail.durability, detail2.durability), true);
            MyEmbedBuilder.AddField("Weight", ComparisonFormatterGreaterThan(detail.weight, detail2.weight), true);
            MyEmbedBuilder.AddField("Steering", ComparisonFormatterGreaterThan(detail.steering, detail2.steering), true);
            MyEmbedBuilder.AddField("Power", ComparisonFormatterGreaterThan(detail.power, detail2.power), true);
            MyEmbedBuilder.AddField("Speed", ComparisonFormatterGreaterThan(detail.speed, detail2.speed), true);


            await ReplyAsync(embed: MyEmbedBuilder.Build());
        }

        //highlight bigger value
        private string ComparisonFormatterGreaterThan(int a, int b) {
            return a > b ? $"***{a}***\n{b}" : (a == b) ? $"{a}\n{b}" : $"{a}\n***{b}***";
        }
        //highlight lower value
        private string ComparisonFormatterLesserThan(int a, int b) {
            return a < b ? $"***{a}***\n{b}" : (a == b) ? $"{a}\n{b}" : $"{a}\n***{b}***";
        }
    }
}
