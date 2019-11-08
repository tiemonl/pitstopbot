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
using System.Diagnostics;

namespace PitStopBot.Commands {
    [Group("user")]
    public class UserInfo : ModuleBase {

        public EmbedBuilder MyEmbedBuilder = new EmbedBuilder();
        public string partRarities = "CREL"; //common, rare, epic, legendary
        private string emptyAddress = "0x0000000000000000000000000000000000000000";
        private string ensUrl = "https://manager.ens.domains/name/";
        private AddressUtil addressUtil = new AddressUtil();
        private readonly Logger logger = new Logger();
        public UserInfoUtils userUtils = new UserInfoUtils();
        public UserInfo() {
        }

        public async Task<string> GetFormattedAddress(string addressInput) {
            Stopwatch sw = Stopwatch.StartNew();
            string addressToFormat;
            if (addressInput.Contains(".eth")) {
                EnsRepository ensRepo = new EnsRepository();
                var ens = await ensRepo.GetENS(addressInput);
                addressToFormat = ens.address;
            } else {
                addressToFormat = addressInput;
            }

            sw.Stop();
            await logger.Log(new LogMessage(LogSeverity.Warning, GetType().FullName, $"Time taken: {sw.Elapsed.TotalMilliseconds}ms"));
            return addressUtil.ConvertToChecksumAddress(addressToFormat);
        }

        [Command("rarities"), Summary("rarities shows how many parts a user has in each rarity category (Legendary, Epic, Rare, Common)")]
        public async Task GetRarities([Summary("User's eth adress")] string addressInput) {
            var address = await GetFormattedAddress(addressInput);
            if (address.Equals(emptyAddress)) {
                await ReplyAsync($"Invalid ENS. Please make sure to set the resolver and then your address in the domains.\n" +
                    $"Do so here: {ensUrl}{addressInput}");
                return;
            }
            Inventory inv = await userUtils.GetInventory(address);

            MyEmbedBuilder = userUtils.GetInventoryRarities(inv);
            MyEmbedBuilder.WithTitle("Part Rarity Distribution");

            await ReplyAsync(embed: MyEmbedBuilder.Build());
        }

        [Command("types"), Summary("types shows how many parts a user has in each type category (Wheels, Body, Rear, Front)")]
        public async Task GetTypes([Summary("User's eth adress")] string addressInput) {
            var address = await GetFormattedAddress(addressInput);
            Inventory inv = await userUtils.GetInventory(address);
            MyEmbedBuilder = userUtils.GetInventoryTypes(inv);
            MyEmbedBuilder.WithTitle("Part Type Distribution");
            await ReplyAsync(embed: MyEmbedBuilder.Build());
        }

        [Command("brands"), Summary("Brands shows how many parts a user has in each brand category (Bolt, Guerilla, Hyperion, Python, Vista, Zeta).")]
        public async Task GetBrands([Summary("User's eth adress")] string addressInput) {
            var address = await GetFormattedAddress(addressInput);
            Inventory inv = await userUtils.GetInventory(address);

            MyEmbedBuilder = userUtils.GetInventoryBrands(inv);
            MyEmbedBuilder.WithTitle("Brand Distribution");

            await ReplyAsync(embed: MyEmbedBuilder.Build());
        }

        [Group("elites"), Summary("Elites commands return inventory stats based on elite parts only.")]
        class ElitesInventory : UserInfo {

            [Command, Summary("Shows how many parts a user has that are ***elite***.")]
            public async Task GetEliteCount([Summary("User's eth adress")] string addressInput) {
                var address = await GetFormattedAddress(addressInput);
                Inventory inv = await userUtils.GetInventory(address);

                MyEmbedBuilder.WithTitle("Elite Parts:");
                MyEmbedBuilder.WithColor(Color.LightGrey);

                MyEmbedBuilder.AddField("Count", inv.parts.Count(i => i.details.isElite), true);
                await ReplyAsync(embed: MyEmbedBuilder.Build());
            }

            [Command("brands"), Summary("Brands shows how many parts a user has in each brand category (Bolt, Guerilla, Hyperion, Python, Vista, Zeta).")]
            public async Task GetEliteBrands([Summary("User's eth adress")] string addressInput) {
                var address = await GetFormattedAddress(addressInput);
                Inventory inv = await userUtils.GetInventory(address);

                MyEmbedBuilder = userUtils.GetInventoryBrands(inv, true);
                MyEmbedBuilder.WithTitle("Elite Brand Distribution");

                await ReplyAsync(embed: MyEmbedBuilder.Build());
            }

            [Command("types"), Summary("types shows how many parts a user has in each type category (Wheels, Body, Rear, Front)")]
            public async Task GetEliteTypes([Summary("User's eth adress")] string addressInput) {
                var address = await GetFormattedAddress(addressInput);
                Inventory inv = await userUtils.GetInventory(address);

                MyEmbedBuilder = userUtils.GetInventoryTypes(inv, true);
                MyEmbedBuilder.WithTitle("Elite Type Distribution");

                await ReplyAsync(embed: MyEmbedBuilder.Build());
            }

            [Command("rarities"), Summary("rarities shows how many parts a user has in each rarity category (Legendary, Epic, Rare, Common)")]
            public async Task GetEliteRarities([Summary("User's eth adress")] string addressInput) {
                var address = await GetFormattedAddress(addressInput);
                Inventory inv = await userUtils.GetInventory(address);

                MyEmbedBuilder = userUtils.GetInventoryRarities(inv, true);
                MyEmbedBuilder.WithTitle("Elite Part Rarity Distribution");

                await ReplyAsync(embed: MyEmbedBuilder.Build());
            }
        }

        [Group("cars"), Summary("Cars is used to determined what cars a user can make with their parts.")]
        class CarMaker : UserInfo {
            [Command("model", RunMode = RunMode.Async), Summary("Model returns a list of complete cars that can be built by same model parts within the same brand." +
                "\nFor example Zeta MX Wheels, Body, Rear and Front")]
            public async Task GetCarsByModel([Summary("rarity of parts that should be used to build the car (Legendary, Epic, Rare, Common, Any)")]string rarity,
                [Summary("User's eth adress")] string addressInput) {
                var address = await GetFormattedAddress(addressInput);
                Inventory inv = await userUtils.GetInventory(address);

                char rarityChosen = rarity.ToUpper()[0];
                Dictionary<string, List<string>> completeCars = new Dictionary<string, List<string>>();
                var parts = inv.parts;
                var rarityParts = partRarities.Contains(rarityChosen) ? parts.Where(p => p.details.rarity.StartsWith(rarityChosen)).ToList() : parts;
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
                    MyEmbedBuilder.AddField(brand.Key, sb.ToString(), true);
                    sb.Clear();
                }
                MyEmbedBuilder.WithTitle("Buildable car List built by model");
                MyEmbedBuilder.WithColor(Color.DarkTeal);
                await logger.Log(new LogMessage(LogSeverity.Critical, "", "test"));
                await ReplyAsync(embed: MyEmbedBuilder.Build());
            }

            [Command("brand", RunMode = RunMode.Async), Summary("returns a list of complete cars that can be built within the same brand.\n" +
                "For example Zeta RX Wheels, Zeta GX Body, Zeta MX Rear and Front")]
            public async Task GetCarsByBrand([Summary("rarity of parts that should be used to build the car (Legendary, Epic, Rare, Common, Any)")]string rarity,
                [Summary("User's eth adress")] string addressInput) {
                var address = await GetFormattedAddress(addressInput);
                Inventory inv = await userUtils.GetInventory(address);

                char rarityChosen = rarity.ToUpper()[0];
                Dictionary<string, int> completeCars = new Dictionary<string, int>();
                var parts = inv.parts;
                var rarityParts = partRarities.Contains(rarityChosen) ? parts.Where(p => p.details.rarity.StartsWith(rarityChosen)).ToList() : parts;
                var listOfBrands = rarityParts.GroupBy(e => e.details.brand).Select(g => g.ToList()).ToList();
                foreach (var partsOfBrand in listOfBrands) {
                    var typePerBrand = partsOfBrand.GroupBy(e => e.details.type).Select(g => g.ToList()).ToList();
                    var carTotal = typePerBrand.Count() == 4 ? typePerBrand.Min(e => e.Count()) : 0;
                    if (carTotal != 0) {
                        var brandKey = typePerBrand[0][0].details.brand;
                        completeCars.Add(brandKey, carTotal);
                    }
                }
                foreach (var brand in completeCars) {
                    MyEmbedBuilder.AddField(brand.Key, $"{brand.Value}x", true);
                }
                MyEmbedBuilder.WithTitle("Car List buildable by brand");
                MyEmbedBuilder.WithColor(Color.DarkTeal);
                await ReplyAsync(embed: MyEmbedBuilder.Build());
            }

            [Command("any", RunMode = RunMode.Async), Summary("returns a list of cars that can be built with any four parts.\n" +
                "For example Zeta RX Wheels, Guerilla Bravo Body, Hyperion Spirit Rear and Python Rush Front")]
            public async Task GetCarsByAny([Summary("rarity of parts that should be used to build the car (Legendary, Epic, Rare, Common, Any)")] string rarity,
                [Summary("User's eth adress")] string addressInput) {
                var address = await GetFormattedAddress(addressInput);
                Inventory inv = await userUtils.GetInventory(address);

                char rarityChosen = rarity.ToUpper()[0];

                var parts = inv.parts;
                var rarityParts = partRarities.Contains(rarityChosen) ? parts.Where(p => p.details.rarity.StartsWith(rarityChosen)).ToList() : parts;
                var types = rarityParts.GroupBy(e => e.details.type).Select(g => g.ToList()).ToList();
                var completeCars = types.Count() == 4 ? types.Min(e => e.Count()) : 0;

                MyEmbedBuilder.AddField("Total buildable cars", completeCars == 1 ? $"{completeCars} car" : $"{completeCars} cars", true);

                MyEmbedBuilder.WithColor(Color.DarkTeal);
                await ReplyAsync(embed: MyEmbedBuilder.Build());
            }
        }
    }
}
