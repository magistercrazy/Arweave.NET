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
        public static byte[] DeepHashChunks(List<object> chunk, byte[] acc)
        {            
            if (chunk.Count < 1)
                return acc;


            var hashPair = Utils.ConcatBuffers(acc, DeepHash(chunk[0]));
            var newAcc = Encryption.Hash(hashPair, "SHA-384");
            chunk.RemoveAt(0);
            return DeepHashChunks(chunk, newAcc);
        }

        public static byte[] DeepHash(object data)
        {

            if (data.GetType().IsArray)
            {
                var byteArr = (byte[])data;
                var tag = Utils.ConcatBuffers(
                    Encoding.UTF8.GetBytes("blob"),
                    Encoding.UTF8.GetBytes(byteArr.Length.ToString())
                    );

                var taggedHash = Utils.ConcatBuffers(

                    Encryption.Hash(tag, "SHA-384"),
                    Encryption.Hash(byteArr, "SHA-384")
                );

                return Encryption.Hash(taggedHash, "SHA-384");
            }
            else
            {

                var byteLst = (List<object>)data;
                var tag = Utils.ConcatBuffers(
                    Encoding.UTF8.GetBytes("list"),
                    Encoding.UTF8.GetBytes(byteLst.Count.ToString())
                    );
                return DeepHashChunks(byteLst, Encryption.Hash(tag, "SHA-384"));
            }

        }
    }
}
