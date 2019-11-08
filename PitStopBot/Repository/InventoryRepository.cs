using System.Threading.Tasks;
using PitStopBot.Objects;
using PitStopBot.Utils;

namespace Repository {
    public class InventoryRepository {
        private async Task<Inventory> GetInventory(string apiUrl, string address, int lastToken) {
            var apiUrlFull = string.Format(apiUrl, address, lastToken);
            return await new ApiUtils().CallApiAsync<Inventory>(apiUrlFull);
        }
    }
}
