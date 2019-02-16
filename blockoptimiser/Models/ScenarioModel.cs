using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class ScenarioModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public String Name { get; set; }
        public int StartYear { get; set; }
        public int TimePeriod { get; set; }
        public int DiscountFactor { get; set; }
    }
}
