using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AmmResearch.Models
{
    public class Token
    {
        public string Symbol { get; set; }
        [JsonProperty("contract_addr")]
        public string Address { get; set; }
        public int Decimals { get; set; }
        [JsonProperty("token_code_hash")]
        public string CodeHash { get; set; }
        [JsonProperty("viewing_key")]
        public string ViewingKey { get; set; }
    }
    public class Pair
    {
        [JsonProperty("contract_addr")]
        public string Address { get; set; }
        public Token Token0 { get; set; }
        public Token Token1 { get; set; }
        public BigInteger Volume0 { get; set; }
        public BigInteger Volume1 { get; set; }
    }
}
