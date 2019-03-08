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
}
