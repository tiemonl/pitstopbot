using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;
using PitStopBot.Objects;

namespace PitStopBot.Utils {
    public class UserInfoUtils {
        private readonly string apiLink = "https://battleracers.io/api/getParts?address={0}&lastTokenId={1}";
        private readonly Logger logger = new Logger();
        private EmbedBuilder embedBuilder = new EmbedBuilder();
        public string partRarities = "CREL"; //common, rare, epic, legendary
        public async Task<Inventory> GetInventory(string address) {
            Inventory inventory = null;
            int lastToken = 1;
            Stopwatch sw = Stopwatch.StartNew();
            inventory = await callApi(apiLink, address, lastToken);
            sw.Stop();
            await logger.Log(new Discord.LogMessage(Discord.LogSeverity.Warning, address, $"time to get inventory: {sw.ElapsedMilliseconds} ms"));
            return inventory;
        }

        private async Task<Inventory> callApi(string api, string address, int lastToken) {
            Inventory inv = null;
            var apiLinkFull = string.Format(api, address, lastToken);
            using (var client = new HttpClient()) {
                using (var response = client.GetAsync(apiLinkFull).Result) {
                    if (response.IsSuccessStatusCode) {
                        string invJson = await response.Content.ReadAsStringAsync();
                        inv = JsonConvert.DeserializeObject<Inventory>(invJson);
                        if (inv.parts.Any()) {
                            lastToken = inv.parts.Max(i => int.Parse(i.id));
                            Inventory tempInv = await callApi(api, address, lastToken);
                            inv.parts.AddRange(tempInv.parts);
                        }
                    } else {
                        Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    }
                }
            }
            return inv;
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
                embedBuilder.AddField(StringUtils.RenameType(typeName),
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
