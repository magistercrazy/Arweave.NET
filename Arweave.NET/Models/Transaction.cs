using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Arweave.NET.Models
{
    public class Transaction:BaseModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("last_tx")]
        public string LastTx { get; set; }
        [JsonPropertyName("owner")]
        public string Owner { get; set; }
        [JsonPropertyName("tags")]
        public object[] Tags { get; set; }
        [JsonPropertyName("target")]
        public string Target { get; set; }
        [JsonPropertyName("quantity")]
        public string Quantity { get; set; }
        [JsonPropertyName("data")]
        public string Data { get; set; }
        [JsonPropertyName("reward")]
        public string Reward { get; set; }
        [JsonPropertyName("signature")]
        public string Signature { get; set; }

       
    }
    

}
