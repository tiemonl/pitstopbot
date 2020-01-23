using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;
using PitStopBot.Objects;

namespace PitStopBot.Utils {
    public class PartInfoUtils {

        public string partRarities = "CREL"; //common, rare, epic, legendary
        private readonly Color legendaryColor = new Color(196, 181, 59);
        private readonly Color epicColor = new Color(133, 212, 225);
        private readonly Color rareColor = new Color(218, 146, 65);
        private readonly Color commonColor = new Color(168, 161, 174);

        public async Task<Part> GetPart(string num) {
            Part part = null;
            var apiLink = $"https://battleracers.io/api/items/{num}?address=0x";
            using (var client = new HttpClient()) {
                using (var response = client.GetAsync(apiLink).Result) {
                    if (response.IsSuccessStatusCode) {
                        string invJson = await response.Content.ReadAsStringAsync();
                        part = JsonConvert.DeserializeObject<Part>(invJson);
                    } else {
                        Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    }
                }
            }
            return part;
        }

        public List<Part> GetPartsByRarity(string rarity, List<Part> parts) {
            char rarityChosen = rarity.ToUpper()[0];
            return partRarities.Contains(rarityChosen) ? parts.Where(p => p.details.rarity.StartsWith(rarityChosen)).ToList() : parts;
        }

        public Color GetEmbedColorByRarity(Part part) =>
                part.details.rarity.Equals("Legendary") ? legendaryColor :
                part.details.rarity.Equals("Epic") ? epicColor :
                part.details.rarity.Equals("Rare") ? rareColor : commonColor;

    }
}
