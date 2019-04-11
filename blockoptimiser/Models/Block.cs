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
    }

    public class BlockPosition
    {
        public long Bid { get; set; }
        public int I { get; set; }
        public int J { get; set; }
        public int K { get; set; }

    }
}
