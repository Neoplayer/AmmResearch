using Newtonsoft.Json;
using System.Collections.Generic;

namespace AmmResearch
{
    public class SecretSwapPairsResponse
    {
        public List<RespPair> Pairs { get; set; }
        public class RespPair
        {
            [JsonProperty("contract_addr")]
            public string Address { get; set; }
            [JsonProperty("asset_infos")]
            public List<AssetInfo> AssetInfos { get; set; }
            [JsonProperty("token_code_hash")]
            public string TokenCodeHash { get; set; }

            public class RespToken
            {             
                [JsonProperty("contract_addr")]
                public string Address { get; set; }             
                [JsonProperty("token_code_hash")]
                public string CodeHash { get; set; }
                [JsonProperty("viewing_key")]
                public string ViewingKey { get; set; }
            }
            public class AssetInfo
            {
                [JsonProperty("token")]
                RespToken Token { get; set; }

            }
        }
    }
}