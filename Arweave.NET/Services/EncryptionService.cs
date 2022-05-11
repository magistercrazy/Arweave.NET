using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Arweave.NET.Services
{
    public class EncryptionService
    {
        public byte[] Hash(byte[] data, string alg)
        {
            var algorithm = HashAlgorithm.Create(alg);
            var hashByteArray = algorithm.ComputeHash(data);
            return hashByteArray;
        }
    }
}
