using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
