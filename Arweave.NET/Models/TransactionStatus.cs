using System.Text.Json.Serialization;

namespace Arweave.NET.Models
{
    public class TransactionStatus:BaseModel
    {
        [JsonPropertyName("block_height")]
        public long BlockHeight { get; set; }
        [JsonPropertyName("block_indep_hash")]
        public string BlockIndepHash { get; set; }
        [JsonPropertyName("number_of_confirmations")]
        public long NumOfConfirmations { get; set; }
    }
}
