using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PitStopBot.Objects;
using PitStopBot.Utils;

namespace PitStopBot.Repository {
    public class EnsRepository {
        private readonly GetKey getKey;
        public EnsRepository() {
            getKey = new GetKey();
        }

        public async Task<Ens> GetENS(string domain) {
            var apiLink = $"https://cindercloud.p.rapidapi.com/api/ethereum/ens/resolve/{domain}";
            var headers = new Dictionary<String, String>();
            headers.Add("X-RapidAPI-Host", "cindercloud.p.rapidapi.com");
            headers.Add("X-RapidAPI-Key", getKey.GetAPI("rapidapi"));
            return await new ApiUtils().CallApiAsync<Ens>(apiLink, headers);
        }
    }
}