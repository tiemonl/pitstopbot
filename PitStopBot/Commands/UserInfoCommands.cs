using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PitStopBot.Objects;
using PitStopBot.Utils;
using Nethereum.Util;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PitStopBot.Commands {
    [Group("user")]
    public class UserInfo : ModuleBase {

        public EmbedBuilder MyEmbedBuilder = new EmbedBuilder();
        public string partRarities = "CREL"; //common, rare, epic, legendary
        private string emptyAddress = "0x0000000000000000000000000000000000000000";
        private string ensUrl = "https://manager.ens.domains/name/";
        private AddressUtil addressUtil = new AddressUtil();

        public UserInfoUtils userUtils = new UserInfoUtils();
        public UserInfo() {
        }

        public async Task<string> GetFormattedAddress(string addressInput) {
            string addressToFormat = null;
            if (addressInput.Contains(".eth")) {
                EnsUtils ensUtil = new EnsUtils();
                var ens = await ensUtil.GetENS(addressInput);
                addressToFormat = ens.address;
            } else {
                addressToFormat = addressInput;
            }
            return addressUtil.ConvertToChecksumAddress(addressToFormat);
        }

        [Command("rarities"), Summary("returns the parts count")]
        public async Task GetRarities([Summary("User's eth adress")] string addressInput) {
            var address = await GetFormattedAddress(addressInput);
            if (address.Equals(emptyAddress)) {
                await ReplyAsync($"Invalid ENS. Please make sure to set the resolver and then your address in the domains.\n" +
                    $"Do so here: {ensUrl}{addressInput}");
                return;
            }
            Inventory inv = await userUtils.GetInventory(address);
            var parts = inv.parts;
            var rarityList = new List<string>();
            foreach (var p in parts) {
                rarityList.Add(p.details.rarity);
            }
            MyEmbedBuilder.WithTitle("Part Rarity Distribution");
            MyEmbedBuilder.WithColor(Color.Blue);

            var orderDict = partRarities.Select((c, i) => new { Letter = c, Order = i })
                                 .ToDictionary(o => o.Letter, o => o.Order);
            var rarities = rarityList.OrderBy(i => orderDict[i[0]]).GroupBy(i => i);
            foreach (var r in rarities) {
                MyEmbedBuilder.AddField(r.Key, r.Count(), true);
            }

            MyEmbedBuilder.AddField("Total Parts", inv.total, false);
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
                    default:
                        break;
                }
            }
            MyEmbedBuilder.WithTitle("Part Type Distribution");
            MyEmbedBuilder.WithColor(Color.Green);
            MyEmbedBuilder.AddField("Wheels", wheels, true);
            MyEmbedBuilder.AddField("Front", bumper, true);
            MyEmbedBuilder.AddField("Rear", spoiler, true);
            MyEmbedBuilder.AddField("Body", casing, true);
            MyEmbedBuilder.AddField("Total Parts", inv.total, false);
            await ReplyAsync(embed: MyEmbedBuilder.Build());
        }
        [Command("elites"), Summary("returns the parts count")]
        public async Task GetEliteCount([Summary("User's eth adress")] string addressInput) {
            var address = await GetFormattedAddress(addressInput);
            Inventory inv = await userUtils.GetInventory(address);

            MyEmbedBuilder.WithTitle("Elite Parts:");
            MyEmbedBuilder.WithColor(Color.LightGrey);
            MyEmbedBuilder.AddField("Count", inv.parts.Count(i => i.details.isElite), true);
            await ReplyAsync(embed: MyEmbedBuilder.Build());
        }

        [Command("brands"), Summary("returns the brand count")]
        public async Task GetBrands([Summary("User's eth adress")] string addressInput) {
            var address = await GetFormattedAddress(addressInput);
            Inventory inv = await userUtils.GetInventory(address);
            var parts = inv.parts;
            var brandList = new List<string>();
            foreach (var p in parts) {
                brandList.Add(p.details.brand);
            }
            MyEmbedBuilder.WithTitle("Brand Distribution");
            MyEmbedBuilder.WithColor(Color.DarkMagenta);
            var brands = brandList.GroupBy(i => i).OrderBy(i => i.Key);
            foreach (var b in brands) {
                MyEmbedBuilder.AddField(b.Key, b.Count(), true);
            }
            MyEmbedBuilder.AddField("Total Parts", inv.total, false);
            await ReplyAsync(embed: MyEmbedBuilder.Build());
        }
        [Group("cars"), Summary("finds out which cars can be made from parts")]
        public class CarMaker : ModuleBase<SocketCommandContext> {
            private UserInfo userInfo;
            public CarMaker(UserInfo userInfo) {
                this.userInfo = userInfo;
            }


            [Command("model", RunMode = RunMode.Async), Summary("returns a list of complete cars that can be built.")]
            public async Task GetCars([Summary("rarity of the car being built")]string rarity, [Summary("User's eth adress")] string addressInput) {
                var address = await userInfo.GetFormattedAddress(addressInput);
                Inventory inv = await userInfo.userUtils.GetInventory(address);

                char rarityChosen = rarity.ToUpper()[0];
                Dictionary<string, List<string>> completeCars = new Dictionary<string, List<string>>();
                var parts = inv.parts;
                var rarityParts = userInfo.partRarities.Contains(rarityChosen) ? parts.Where(p => p.details.rarity.StartsWith(rarityChosen)).ToList() : parts;
                var listOfBrands = rarityParts.GroupBy(e => e.details.brand).Select(g => g.ToList()).ToList();
                foreach (var brand in listOfBrands) {
                    var models = brand.GroupBy(e => e.details.model).Select(g => g.ToList()).ToList();
                    foreach (var model in models) {
                        var types = model.GroupBy(e => e.details.type).Select(g => g.ToList()).ToList();

                        if (types.Count() == 4) {
                            var brandKey = types[0][0].details.brand;
                            var modelKey = types[0][0].details.model;
                            if (!completeCars.ContainsKey(brandKey)) {
                                completeCars.Add(brandKey, new List<string>());
                            }
                            completeCars[brandKey].Add($"{brandKey} {modelKey}");
                        }
                    }
                }
                StringBuilder sb = new StringBuilder();
                foreach (var brand in completeCars) {
                    foreach (var model in brand.Value) {
                        sb.Append(model + "\n");
                    }
                    userInfo.MyEmbedBuilder.AddField(brand.Key, sb.ToString(), true);
                    sb.Clear();
                }
                userInfo.MyEmbedBuilder.WithTitle("Car List built by model");
                userInfo.MyEmbedBuilder.WithColor(Color.DarkTeal);

                await ReplyAsync(embed: userInfo.MyEmbedBuilder.Build());
            }
        }

    }
}
