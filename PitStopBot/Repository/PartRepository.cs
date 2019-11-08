using System.Threading.Tasks;
using PitStopBot.Objects;
using PitStopBot.Utils;

namespace PitStopBot.Repository {
    public class PartRepository {
        public async Task<Part> GetPart(string num) {
            ApiUtils api = new ApiUtils();
            var apiLink = $"https://battleracers.io/api/items/{num}";
            return await api.CallApiAsync<Part>(apiLink);
        }
    }
}
