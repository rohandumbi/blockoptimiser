using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class Project
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public String BackgroundColor { get; set; }

        public override string ToString()
        {
            return Name + "," + Description + "," + CreatedDate + "," + ModifiedDate;
        }
    }
}
