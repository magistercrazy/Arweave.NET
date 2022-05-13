
namespace Arweave.NET.Models
{
    public class Chunk
    {
        public byte[] DataHash { get; set; }
        public int MinByteRange { get; set; }
        public int MaxByteRange { get; set; }
    }
}
