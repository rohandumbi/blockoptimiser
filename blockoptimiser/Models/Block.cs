using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class Block
    {
        public long Id { get; set; }
        public IDictionary<string, object> data { get; set; }
        public List<Process> Processes { get; set; }
        public List<Block> DependentBlocks { get; set; }
        public Boolean IsMined { get; set; }
        public Boolean IsIncluded { get; set; }
        public Boolean IsProcessBlock
        {
            get {
                return (Processes != null) && (Processes.Count > 0);
            }
        }
    }

    public class BlockPosition
    {
        public long Bid { get; set; }
        public int I { get; set; }
        public int J { get; set; }
        public int K { get; set; }
        public List<Process> Processes { get; set; }

    }

    public class MinedBlock
    {
        public long Bid { get; set; }
        public int Year { get; set; }
        public int I { get; set; }
        public int J { get; set; }
        public int K { get; set; }

    }
}
