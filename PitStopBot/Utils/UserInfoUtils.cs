using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PitStopBot.Objects;

namespace PitStopBot.Utils {
    public class UserInfoUtils {
        private readonly string apiLink = "https://battleracers.io/api/getParts?address={0}&lastTokenId={1}";
        private readonly Logger logger = new Logger();
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
    }
}
