using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PitStopBot.Objects;

namespace PitStopBot.Utils {
    public class PartInfoUtils {
        public async Task<Part> GetPart(string num) {
            Part part = null;
            var apiLink = $"https://battleracers.io/api/items/{num}";
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
    }
}
