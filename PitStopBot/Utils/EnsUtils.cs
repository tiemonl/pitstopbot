using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PitStopBot.Objects;

namespace PitStopBot.Utils {
    public class EnsUtils {
        private readonly GetKey getKey;
        public EnsUtils() {
            getKey = new GetKey();
        }
        public async Task<ENS> GetENS(string domain) {
            ENS ens = null;
            var baseAddress = $"https://cindercloud.p.rapidapi.com/api/ethereum/ens/resolve/{domain}";
            using (var client = new HttpClient()) {
                client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "cindercloud.p.rapidapi.com");
                client.DefaultRequestHeaders.Add("X-RapidAPI-Key", getKey.GetAPI("rapidapi"));
                using (var response = client.GetAsync(baseAddress).Result) {
                    if (response.IsSuccessStatusCode) {
                        var ensJson = await response.Content.ReadAsStringAsync();
                        ens = JsonConvert.DeserializeObject<ENS>(ensJson);
                    } else {
                        Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    }
                }
            }
            return ens;
        }
    }
}