using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class Process
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public String Name { get; set; }
        public List<ProcessModelMapping> Mapping { get; set; }
        public Boolean CheckStatus { get; set; }
    }

    public class ProcessModelMapping
    {
        public int ProcessId { get; set; }
        public int ModelId { get; set; }
        public String FilterString { get; set; }
    }
}
