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
        public int MaxChunkSize = 256 * 1024;
        public int MinChunkSize = 32 * 1024;
        private readonly Utils _utils = new();

        public byte[] GenerateTransactionChunks(byte[] data)
        {
            var chunks = ChunkData(data);
            var leaves = GenerateLeaves(chunks);
            var root = BuildLayers(leaves);
            return root.Id;
        }


        public List<Chunk> ChunkData(byte[] data)
        {
            var encription = new EncryptionService();
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
                    DataHash = encription.Hash(chunk, "SHA-256"),
                    MinByteRange = cursor - chunk.Length,
                    MaxByteRange = cursor
                });
            }
            chunks.Add(new Chunk
            {
                DataHash = encription.Hash(rest, "SHA-256"),
                MinByteRange = cursor,
                MaxByteRange = cursor + rest.Length
            });

            return chunks;

        }

        public List<LeafNode> GenerateLeaves(List<Chunk> chunks)
        {
            var encription = new EncryptionService();
            var nodes = new List<LeafNode>();
            foreach (var chunk in chunks)
            {
                var leavesHash = encription.Hash(chunk.DataHash, "SHA-256");
                var rangeHash = encription.Hash(_utils.IntToBuffer(chunk.MaxByteRange), "SHA-256");
                var concatated = leavesHash.Concat(rangeHash).ToArray();
                nodes.Add(new LeafNode
                {
                    Id = encription.Hash(concatated, "SHA-256"),
                    DataHash = chunk.DataHash,
                    MinByteRange = chunk.MinByteRange,
                    MaxByteRange = chunk.MaxByteRange
                }) ;
            }
            return nodes;
        }

        public MerkelNode BuildLayers(List<LeafNode> nodes, int level = 0)
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

        public LeafNode HashBranch(LeafNode left, LeafNode right)
        {
            //if (right == null)
            //    return left as BranchNode;

            var encription = new EncryptionService();
            var lHash = encription.Hash(left.Id, "SHA-256");
            var rHash = encription.Hash(right.Id, "SHA-256");
            var rangeHash = encription.Hash(_utils.IntToBuffer(left.MaxByteRange), "SHA-256");
            var concatated = new List<byte>().Concat(lHash).Concat(rHash).Concat(rangeHash).ToArray();
            var res = encription.Hash(concatated, "SHA-256"); 
            
            return new LeafNode() {Id = res };
        }

    }
}
