using Arweave.NET.Services;
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
        public int Format { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("last_tx")]
        public string LastTx { get; set; }
        [JsonPropertyName("owner")]
        public string Owner { get; set; }
        [JsonPropertyName("tags")]
        public List<Tag> Tags { get; set; }
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
        [JsonIgnore]
        public JsonWebKey JWK {get;set;}
        public Transaction()
        {
            Format = 2;
            Tags = new List<Tag>();
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
                JWK = jwks.Keys[0];
            }
        }

        public Transaction(string keyFilePath):base()
        {
            LoadOwner(keyFilePath);
        }

        public void CreateDataTransaction(byte[] buffer)
        {
            Format = 2;
            Quantity = "0";
            Target = "";
            Data = Utils.Base64Encode(buffer);
            DataSize = buffer.Length.ToString();
            DataRoot = Utils.Base64Encode(ChunkService.GenerateTransactionChunks(buffer));
        }

        public void AddTag(string name, string value)
        {
            var tag = new Tag
            {
                Name = Utils.Base64Encode(Encoding.UTF8.GetBytes(name)),
                Value = Utils.Base64Encode(Encoding.UTF8.GetBytes(value))
            };
            Tags.Add(tag);
        }

       
    }
    

}
