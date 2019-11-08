using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PitStopBot.Utils {
    public class ApiUtils {
        public async Task<T> CallApiAsync<T>(string link, Dictionary<String, String> headers = null) {
            T item = default;
            using (var client = new HttpClient()) {
                if (headers.Count > 0)
                    foreach (var header in headers)
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);

                using (var response = client.GetAsync(link).Result) {
                    if (response.IsSuccessStatusCode) {
                        string invJson = await response.Content.ReadAsStringAsync();
                        item = JsonConvert.DeserializeObject<T>(invJson);
                    } else {
                        Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    }
                }
            }
            return item;
        }
    }
}
