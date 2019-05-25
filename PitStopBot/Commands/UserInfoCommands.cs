using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PitStopBot.Objects;
using PitStopBot.Utils;
using Nethereum.Util;

namespace PitStopBot.Commands {
	[Group("user")]
	public class UserInfo : ModuleBase {

		private EmbedBuilder MyEmbedBuilder = new EmbedBuilder();
		private EmbedFieldBuilder MyEmbedField = new EmbedFieldBuilder();

		UserInfoUtils userUtils = new UserInfoUtils();
		public UserInfo() {
		}

		private async Task<string> GetFormattedAddress(string addressInput) {
			string addressToFormat = null;
			if (addressInput.Contains(".eth")) {
				EnsUtils ensUtil = new EnsUtils();
				var ens = await ensUtil.GetENS(addressInput);
				addressToFormat = ens.address;
			} else {
				addressToFormat = addressInput;
			}
			return new AddressUtil().ConvertToChecksumAddress(addressToFormat);
		}

		[Command("rarities"), Summary("returns the parts count")]
		public async Task GetRarities([Summary("User's eth adress")] string addressInput) {
			var address = await GetFormattedAddress(addressInput);
			int common = 0, rare = 0, epic = 0, legendary = 0;
			Inventory inv = await userUtils.GetInventory(address);
			var parts = inv.parts;
			foreach (var p in parts) {
				var rarity = p.details.rarity;
				switch (rarity) {
					case "Common":
						common++;
						break;
					case "Rare":
						rare++;
						break;
					case "Epic":
						epic++;
						break;
					case "Legendary":
						legendary++;
						break;
				}
			}
			MyEmbedBuilder.WithTitle("Part Rarity Distribution");
			MyEmbedBuilder.WithColor(Color.Blue);
			MyEmbedBuilder.AddField("Common", common, true);
			MyEmbedBuilder.AddField("Rare", rare, true);
			MyEmbedBuilder.AddField("Epic", epic, true);
			MyEmbedBuilder.AddField("Legendary", legendary, true);
			await ReplyAsync(embed: MyEmbedBuilder.Build());
		}

		[Command("parts"), Summary("returns the parts count")]
		public async Task GetParts([Summary("User's eth adress")] string addressInput) {
			var address = await GetFormattedAddress(addressInput);
			int wheels = 0, bumper = 0, spoiler = 0, casing = 0;
			Inventory inv = await userUtils.GetInventory(address);
			var parts = inv.parts;
			foreach (var p in parts) {
				var type = p.details.type;
				switch (type) {
					case "wheels":
						wheels++;
						break;
					case "bumper":
						bumper++;
						break;
					case "spoiler":
						spoiler++;
						break;
					case "casing":
						casing++;
						break;
				}
			}
			MyEmbedBuilder.WithTitle("Part Type Distribution");
			MyEmbedBuilder.WithColor(Color.Green);
			MyEmbedBuilder.AddField("Wheels", wheels, true);
			MyEmbedBuilder.AddField("Front", bumper, true);
			MyEmbedBuilder.AddField("Rear", spoiler, true);
			MyEmbedBuilder.AddField("Body", casing, true);
			await ReplyAsync(embed: MyEmbedBuilder.Build());
		}
		[Command("elites"), Summary("returns the parts count")]
		public async Task GetEliteCount([Summary("User's eth adress")] string addressInput) {
			var address = await GetFormattedAddress(addressInput);
			int elite = 0;
			Inventory inv = await userUtils.GetInventory(address);
			var parts = inv.parts;
			foreach (var p in parts) {
				if (p.details.isElite) elite++;
			}
			MyEmbedBuilder.WithTitle("Elite Parts:");
			MyEmbedBuilder.WithColor(Color.LightGrey);
			MyEmbedBuilder.AddField("Count", elite, true);
			await ReplyAsync(embed: MyEmbedBuilder.Build());
		}

		[Command("brands"), Summary("returns the brand count")]
		public async Task GetBrands([Summary("User's eth adress")] string addressInput) {
			var address = await GetFormattedAddress(addressInput);
			int bolt = 0, guerilla = 0, hyperion = 0, python = 0, vista = 0, zeta = 0;
			Inventory inv = await userUtils.GetInventory(address);
			var parts = inv.parts;
			foreach (var p in parts) {
				var type = p.details.brand;
				switch (type) {
					case "Bolt":
						bolt++;
						break;
					case "Guerilla":
						guerilla++;
						break;
					case "Hyperion":
						hyperion++;
						break;
					case "Python":
						python++;
						break;
					case "Vista":
						vista++;
						break;
					case "Zeta":
						zeta++;
						break;
				}
			}
			MyEmbedBuilder.WithTitle("Brand Distribution");
			MyEmbedBuilder.WithColor(Color.DarkMagenta);
			MyEmbedBuilder.AddField("Bolt", bolt, true);
			MyEmbedBuilder.AddField("Guerilla", guerilla, true);
			MyEmbedBuilder.AddField("Hyperion", hyperion, true);
			MyEmbedBuilder.AddField("Python", python, true);
			MyEmbedBuilder.AddField("Vista", vista, true);
			MyEmbedBuilder.AddField("Zeta", zeta, true);
			await ReplyAsync(embed: MyEmbedBuilder.Build());
		}
	}
}
