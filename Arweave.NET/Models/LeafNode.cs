using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arweave.NET.Models
{
    public class LeafNode : MerkelNode
    {
        public byte[] DataHash { get; set; }
        public int MinByteRange { get; set; }
        public int MaxByteRange { get; set; }
    }
}
