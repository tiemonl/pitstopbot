using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PitStopBot.Utils {
    public class EnsUtils {
        public EnsUtils() {
        }
        public async Task<string> GetENS(string domain) {
            string address = null;
            var baseAddress = $"https://api.whoisens.org/name/owner/{domain}";
            using (var client = new HttpClient()) {
                using (var response = client.GetAsync(baseAddress).Result) {
                    if (response.IsSuccessStatusCode) {
                        var ensJson = await response.Content.ReadAsStringAsync();
                        dynamic apiRespone = JsonConvert.DeserializeObject(ensJson);
                        address = apiRespone.result.result;
                    } else {
                        Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    }
                }
            }
            return address;
        }
    }
}
