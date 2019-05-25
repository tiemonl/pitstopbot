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
		private Color legendaryColor = new Color(196,181,59);
		private Color epicColor = new Color(133, 212, 225);
		private Color rareColor = new Color(218, 146, 65);
		private Color commonColor = new Color(168, 161, 174);

		public PartInfoCommands() {
		}
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
			MyEmbedBuilder.AddField("Is Elite?", detail.isElite, true);
			MyEmbedBuilder.AddField("Serial Number", detail.serialNumber, true);
			MyEmbedBuilder.AddField("Durability", detail.durability, true);
			MyEmbedBuilder.AddField("Weight", detail.weight, true);
			MyEmbedBuilder.AddField("Steering", detail.steering, true);
			MyEmbedBuilder.AddField("Model", detail.model, true);
			MyEmbedBuilder.AddField("Power", detail.power, true);
			MyEmbedBuilder.AddField("Type", detail.type, true);
			MyEmbedBuilder.AddField("Speed", detail.speed, true);
			MyEmbedBuilder.AddField("Rarity", detail.rarity, true);

			await ReplyAsync(embed: MyEmbedBuilder.Build());
		}
	}
}
