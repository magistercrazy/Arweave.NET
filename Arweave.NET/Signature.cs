using Arweave.NET.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arweave.NET
{
    public class Signature
    {
        private readonly Utils _utils = new();
        private readonly HashChunks _hashChunks = new();       
        public byte[] GetSignature(Transaction transaction)
        {
            var buffer = new List<object>()
            {
                Encoding.UTF8.GetBytes(transaction.Format.ToString()),
                Convert.FromBase64String(_utils.Base64Decode(transaction.Owner)),
                Convert.FromBase64String(_utils.Base64Decode(transaction.Target)),
                Encoding.UTF8.GetBytes(transaction.Quantity),
                Encoding.UTF8.GetBytes(transaction.Reward),
                Convert.FromBase64String(_utils.Base64Decode(transaction.LastTx)),
                _utils.PrepareTags(transaction.Tags),
                Encoding.UTF8.GetBytes(transaction.DataSize),
                Convert.FromBase64String(_utils.Base64Decode(transaction.DataRoot))
            };

            var res = _hashChunks.DeepHash(buffer);
            return res;

        }

        public byte[] Sign(byte[] data, JsonWebKey webKey)
        {
            var jwk = new Jose.Jwk(webKey.E, webKey.N, webKey.P, webKey.Q, webKey.D, webKey.DP, webKey.DQ, webKey.QI);
            var rsa = jwk.RsaKey();

            var algo = new Jose.RsaPssUsingSha(32);
            var res = algo.Sign(data, rsa);

            return res;
        }
    }
}
