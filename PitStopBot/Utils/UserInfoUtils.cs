using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PitStopBot.Objects;

namespace PitStopBot.Utils {
	public class UserInfoUtils {
		public UserInfoUtils() {
		}

		public async Task<Inventory> GetInventory(string address) {
			Inventory inv = null;
			var apiLink = $"https://battleracers.io/api/getParts?address={address}";
			using (var client = new HttpClient()) {
				using (var response = client.GetAsync(apiLink).Result) {
					if (response.IsSuccessStatusCode) {
						string invJson = await response.Content.ReadAsStringAsync();
						inv = JsonConvert.DeserializeObject<Inventory>(invJson);
					} else {
						Console.WriteLine("{0} ({1})", (int) response.StatusCode, response.ReasonPhrase);
					}
				}
			}
			return inv;
		}
	}
}
