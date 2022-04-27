using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
