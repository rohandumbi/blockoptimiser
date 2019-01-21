using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class ModelDimension
    {
        public int Id { get; set; }
        public int ModelId { get; set; }
        public String Type { get; set; }
        public Decimal XDim { get; set; }
        public Decimal YDim { get; set; }
        public Decimal ZDim { get; set; }
    }
}
