using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class ProductJoin
    {
        public int ProjectId { get; set; }
        public String Name { get; set; }
        public List<String> ProductNames { get; set; }      
    }
}
