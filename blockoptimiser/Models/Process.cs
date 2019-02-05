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
        public int FieldId { get; set; }
        public String FilterString { get; set; }
    }
}
