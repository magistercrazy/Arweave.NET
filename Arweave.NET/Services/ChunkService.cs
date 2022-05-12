using Arweave.NET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arweave.NET.Services
{
    public class ChunkService  
    {
        private static readonly int MaxChunkSize = 256 * 1024;
        private static readonly int MinChunkSize = 32 * 1024;

        public static byte[] GenerateTransactionChunks(byte[] data)
        {
            var chunks = ChunkData(data);
            var leaves = GenerateLeaves(chunks);
            var root = BuildLayers(leaves);
            return root.Id;
        }


        private static List<Chunk> ChunkData(byte[] data)
        {
            var chunks = new List<Chunk>();
            var rest = data;
            var cursor = 0;
            while(rest.Length >= MaxChunkSize)
            {
                var chunkSize = MaxChunkSize;
               
                var nextChunkSize = rest.Length - MaxChunkSize;
                if(nextChunkSize > 0 && nextChunkSize < MinChunkSize)
                {
                   var res = Math.Ceiling(Convert.ToDouble(rest.Length) / 2);
                   chunkSize = Convert.ToInt32(res);
                }
                var chunk = rest.Take(chunkSize).ToArray();
                
                rest = rest.Skip(chunkSize).ToArray();
                cursor += chunk.Length;
                chunks.Add(new Chunk
                {
                    DataHash = Encryption.Hash(chunk, "SHA-256"),
                    MinByteRange = cursor - chunk.Length,
                    MaxByteRange = cursor
                });
            }
            chunks.Add(new Chunk
            {
                DataHash = Encryption.Hash(rest, "SHA-256"),
                MinByteRange = cursor,
                MaxByteRange = cursor + rest.Length
            });

            return chunks;

        }

        private static List<LeafNode> GenerateLeaves(List<Chunk> chunks)
        {
            var nodes = new List<LeafNode>();
            foreach (var chunk in chunks)
            {
                var leavesHash = Encryption.Hash(chunk.DataHash, "SHA-256");
                var rangeHash = Encryption.Hash(Utils.IntToBuffer(chunk.MaxByteRange), "SHA-256");
                var concatated = leavesHash.Concat(rangeHash).ToArray();
                nodes.Add(new LeafNode
                {
                    Id = Encryption.Hash(concatated, "SHA-256"),
                    DataHash = chunk.DataHash,
                    MinByteRange = chunk.MinByteRange,
                    MaxByteRange = chunk.MaxByteRange
                }) ;
            }
            return nodes;
        }

        private static MerkelNode BuildLayers(List<LeafNode> nodes, int level = 0)
        {
            if (nodes.Count < 2)
            {
                return nodes.FirstOrDefault();
            }
            if(nodes.Count % 2  > 0)
            {
                nodes.Add(nodes.Last());
            }

            var nextLayer = new List<LeafNode>();
            for (int i = 0; i < nodes.Count; i += 2)
            {
                nextLayer.Add(HashBranch(nodes[i], nodes[i + 1]));
            }

            return BuildLayers(nextLayer, level + 1);
        }

        private static LeafNode HashBranch(LeafNode left, LeafNode right)
        {
            var lHash = Encryption.Hash(left.Id, "SHA-256");
            var rHash = Encryption.Hash(right.Id, "SHA-256");
            var rangeHash = Encryption.Hash(Utils.IntToBuffer(left.MaxByteRange), "SHA-256");
            var concatinated = new List<byte>().Concat(lHash).Concat(rHash).Concat(rangeHash).ToArray();
            var res = Encryption.Hash(concatinated, "SHA-256"); 
            
            return new LeafNode() {Id = res };
        }

    }
}
