using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;
using PitStopBot.Objects;

namespace PitStopBot.Utils {
    public class UserInfoUtils {
        private EmbedBuilder embedBuilder = new EmbedBuilder();
        public string partRarities = "CREL"; //common, rare, epic, legendary
        public async Task<Inventory> GetInventory(string address) {
            Inventory inv = null;
            var apiLink = $"https://battleracers.io/api/getParts?address={address}";
            using (var client = new HttpClient()) {
                using (var response = client.GetAsync(apiLink).Result) {
                    if (response.IsSuccessStatusCode) {
                        string invJson = await response.Content.ReadAsStringAsync();
                        inv = JsonConvert.DeserializeObject<Inventory>(invJson);
                    } else {
                        Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    }
                }
            }
            return inv;
        }

        public async Task<List<Part>> GetEliteInventory(string address) {
            Inventory inv = await GetInventory(address);
            var eliteInv = inv.parts.Where(e => e.details.isElite).ToList();
            return eliteInv;
        }

        public EmbedBuilder GetInventoryRarities(Inventory inv, bool elite = false) {
            var parts = elite ? inv.parts.Where(e => e.details.isElite).ToList() : inv.parts;
            var rarityList = new List<string>();
            foreach (var p in parts) {
                rarityList.Add(p.details.rarity);
            }
            embedBuilder.WithColor(Color.Blue);

            var orderDict = partRarities.Select((c, i) => new { Letter = c, Order = i })
                                 .ToDictionary(o => o.Letter, o => o.Order);
            var rarities = rarityList.OrderBy(i => orderDict[i[0]]).GroupBy(i => i);
            foreach (var r in rarities) {
                embedBuilder.AddField(r.Key, r.Count(), true);
            }

            embedBuilder.AddField("Total Parts", inv.total, false);
            return embedBuilder;
        }

        public EmbedBuilder GetInventoryTypes(Inventory inv, bool elite = false) {
            var parts = elite ? inv.parts.Where(e => e.details.isElite).ToList() : inv.parts;
            var types = parts.GroupBy(e => e.details.type).Select(g => g.ToList()).ToList();
            foreach (var type in types) {
                var typeName = type[0].details.type;
                embedBuilder.AddField(typeName == "wheels" ? "Wheels" :
                                        typeName == "casing" ? "Body" :
                                        typeName == "spoiler" ? "Rear" : "Front",
                                        type.Count(), true);
            }
            embedBuilder.WithColor(Color.Green);

            embedBuilder.AddField("Total Parts", inv.total, false);
            return embedBuilder;
        }

        public EmbedBuilder GetInventoryBrands(Inventory inv, bool elite = false) {
            var parts = elite ? inv.parts.Where(e => e.details.isElite).ToList() : inv.parts;
            var brandList = new List<string>();
            foreach (var p in parts) {
                brandList.Add(p.details.brand);
            }
            embedBuilder.WithColor(Color.DarkMagenta);
            var brands = brandList.GroupBy(i => i).OrderBy(i => i.Key);
            foreach (var b in brands) {
                embedBuilder.AddField(b.Key, b.Count(), true);
            }
            embedBuilder.AddField("Total Parts", inv.total, false);
            return embedBuilder;
        }
    }
}
