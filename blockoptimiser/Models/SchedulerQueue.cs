using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    class SchedulerQueue
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public String FileName { get; set; }
        public int Year { get; set; }
        public int Period { get; set; }
        public Boolean IsProcessed { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
