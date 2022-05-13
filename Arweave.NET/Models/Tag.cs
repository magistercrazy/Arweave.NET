using System.Text.Json.Serialization;

namespace Arweave.NET.Models
{
    public class Tag
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
