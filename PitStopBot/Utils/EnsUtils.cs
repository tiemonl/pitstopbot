using System.Threading.Tasks;
using Newtonsoft.Json;
using PitStopBot.Objects;
using unirest_net.http;

namespace PitStopBot.Utils {
	public class EnsUtils {
		public EnsUtils() {
		}
		public async Task<ENS> GetENS(string domain) {
			var response = await Unirest.get($"https://cindercloud.p.rapidapi.com/api/ethereum/ens/resolve/{domain}")
	.header("X-RapidAPI-Host", "cindercloud.p.rapidapi.com")
	.header("X-RapidAPI-Key", GetKey.GetAPI("rapidapi")).asJsonAsync<string>();
			var body = response.Body;
			var ens = JsonConvert.DeserializeObject<ENS>(body);
			return ens;
		}
	}
}
