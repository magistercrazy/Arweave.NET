using Arweave.NET.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arweave.NET
{
    public class HashChunks
    {
        private readonly Utils _utils = new();
        private readonly EncryptionService _encryption = new();
        public byte[] DeepHashChunks(List<object> chunk, byte[] acc)
        {            
            if (chunk.Count < 1)
                return acc;


            var hashPair = _utils.ConcatBuffers(acc, DeepHash(chunk[0]));
            var newAcc = _encryption.Hash(hashPair, "SHA-384");
            chunk.RemoveAt(0);
            return DeepHashChunks(chunk, newAcc);
        }

        public byte[] DeepHash(object data)
        {

            if (data.GetType().IsArray)
            {
                var byteArr = (byte[])data;
                var tag = _utils.ConcatBuffers(

                Encoding.UTF8.GetBytes("blob"),
                Encoding.UTF8.GetBytes(byteArr.Length.ToString())
            );

                var taggedHash = _utils.ConcatBuffers(

                    _encryption.Hash(tag, "SHA-384"),
                    _encryption.Hash(byteArr, "SHA-384")
                );

                return _encryption.Hash(taggedHash, "SHA-384");
            }
            else
            {

                var byteLst = (List<object>)data;
                var tag = _utils.ConcatBuffers(
                Encoding.UTF8.GetBytes("list"),
                Encoding.UTF8.GetBytes(byteLst.Count.ToString()));
                return DeepHashChunks(byteLst, _encryption.Hash(tag, "SHA-384"));
            }

        }
    }
}
