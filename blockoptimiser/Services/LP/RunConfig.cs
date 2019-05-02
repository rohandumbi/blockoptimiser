using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Services.LP
{
    public class RunConfig
    {
        public int ProjectId { get; set; }
        public int ScenarioId { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public decimal DiscountFactor { get; set; }
    }
}