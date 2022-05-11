using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arweave.NET.Models
{
    public class BranchNode : MerkelNode
    {      
        public int ByteRange { get; set; }
        public int MaxByteRange { get; set; }
        public LeafNode LeftChild { get; set; }
        public LeafNode RightChild { get; set; }
    }
}
