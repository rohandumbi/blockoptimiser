using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class Block
    {
        public int Id { get; set; }
        public IDictionary<string, object> data { get; set; }
    }
}
