using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PitStopBot.Objects;
using PitStopBot.Utils;

namespace PitStopBot.Commands {
    public class PartInfoCommands : ModuleBase {

        private PartInfoUtils partUtils = new PartInfoUtils();
        private EmbedBuilder MyEmbedBuilder = new EmbedBuilder();

        [Command("part"), Summary("Gives information based on the part number.")]
        public async Task GetPartInfo([Summary("Part NFT #")] string num) {
            Part part = await partUtils.GetPart(num);
            Details detail = part.details;

            MyEmbedBuilder.WithTitle("Part Info");
            MyEmbedBuilder.WithColor(partUtils.GetEmbedColorByRarity(part));
            MyEmbedBuilder.WithImageUrl(part.image);
            MyEmbedBuilder.AddField("Brand", detail.brand, true);
            MyEmbedBuilder.AddField("Name", part.name, true);
            MyEmbedBuilder.AddField("Model", detail.model, true);
            MyEmbedBuilder.AddField("Type", StringUtils.RenameType(detail.type), true);
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

        [Command("status"), Summary("Returns the current status of the specified part.")]
        public async Task GetPartStatus([Summary("Part NFT #")] string num) {
            Part part = await partUtils.GetPart(num);
            Details detail = part.details;

            MyEmbedBuilder.WithTitle("Part Status");
            MyEmbedBuilder.WithColor(partUtils.GetEmbedColorByRarity(part));
            MyEmbedBuilder.AddField("Chain", part.chain, true);
            MyEmbedBuilder.AddField("Status", part.state, true);

            await ReplyAsync(embed: MyEmbedBuilder.Build());
        }

        [Command("compare"), Summary("Compares two specified parts information.")]
        public async Task GetPartInfo([Summary("Part NFT #")] string firstPartNum, [Summary("Part NFT #")] string secondPartNum) {
            Part part = await partUtils.GetPart(firstPartNum);
            Part part2 = await partUtils.GetPart(secondPartNum);
            Details detail = part.details;
            Details detail2 = part2.details;

            MyEmbedBuilder.WithTitle("Part Comparison");
            MyEmbedBuilder.WithColor(partUtils.GetEmbedColorByRarity(part));
            MyEmbedBuilder.AddField("Brand", $"{detail.brand}\n{detail2.brand}", true);
            MyEmbedBuilder.AddField("Name", $"{part.name}\n{part2.name}", true);
            MyEmbedBuilder.AddField("Model", $"{detail.model}\n{detail2.model}", true);
            MyEmbedBuilder.AddField("Type", $"{StringUtils.RenameType(detail.type)}\n{StringUtils.RenameType(detail2.type)}", true);
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
