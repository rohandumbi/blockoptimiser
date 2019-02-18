using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class Model
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public String Name { get; set; }
        public Decimal Bearing { get; set; }
        public Boolean HasData { get; set; }
        public Boolean CheckStatus { get; set; }

        /* temp hack. additional fields for product/process mapping */
        public UnitItem AssociatedUnit {get;set;}
        public String FilterString { get; set; }
        public List<UnitItem> AssociatedUnitItems { get; set; }
    }
}
