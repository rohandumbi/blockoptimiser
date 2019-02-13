using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class Product
    {
        public int ProjectId { get; set; }
        public String Name { get; set; }
        public int AssociatedProcessId { get; set; }
        public byte UnitType { get; set; }
        public int UnitId { get; set; }
        public Boolean Check_Status { get; set; }
    }
}
