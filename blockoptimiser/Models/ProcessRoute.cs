using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class ProcessRoute
    {
        public int ProjectId { get; set; }
        public int ProcessId { get; set; }
        public int ParentProcessId { get; set; }
    }
}
