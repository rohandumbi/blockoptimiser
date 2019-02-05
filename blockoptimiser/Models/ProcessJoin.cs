using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class ProcessJoin
    {
        public int ProjectId { get; set; }
        public String Name { get; set; }
        public int ChildProcessId { get; set; }
    }
}
