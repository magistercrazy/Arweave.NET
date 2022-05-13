using System.Security.Cryptography;

namespace Arweave.NET.Services
{
    public class Encryption
    {
        public static byte[] Hash(byte[] data, string alg)
        {
            var algorithm = HashAlgorithm.Create(alg);
            var hashByteArray = algorithm.ComputeHash(data);
            return hashByteArray;
        }
    }
}
