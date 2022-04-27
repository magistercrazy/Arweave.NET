using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Arweave.NET.Models
{
    public class Transaction : BaseModel
    {
        [JsonPropertyName("format")]
        public string Format { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("last_tx")]
        public string LastTx { get; set; }
        [JsonPropertyName("owner")]
        public string Owner { get; set; }
        [JsonPropertyName("tags")]
        public Tag[] Tags { get; set; }
        [JsonPropertyName("target")]
        public string Target { get; set; }
        [JsonPropertyName("quantity")]
        public string Quantity { get; set; }
        [JsonPropertyName("data_root")]
        public string DataRoot { get; set; }
        [JsonPropertyName("data")]
        public string Data { get; set; }
        [JsonPropertyName("data_size")]
        public string DataSize { get; set; }
        [JsonPropertyName("reward")]
        public string Reward { get; set; }
        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        public Transaction()
        {
            Format = "2";
        }

        public void LoadOwner(string keyFilePath)
        {
            string jwksString = string.Empty;
            using (StreamReader sr = File.OpenText(keyFilePath))
            {
                jwksString = sr.ReadToEnd();
                var formattedString = "{ \"keys\":[" + jwksString + "]}";
                var jwks = new JsonWebKeySet(formattedString);
                if (jwks.Keys.Count > 1)
                    throw new NotImplementedException("Key file has more then 1 key, please check");
                Owner = jwks.Keys[0].N;
            }
        }

        public Transaction(string keyFilePath):base()
        {
            LoadOwner(keyFilePath);
        }
       
    }
    

}
