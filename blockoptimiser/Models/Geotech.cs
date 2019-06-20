using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class Geotech
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ModelId { get; set; }
        public int Type { get; set; }
        public int FieldId { get; set; }
        public Boolean UseScript { get; set; }
        public String FieldName { get; set; }
        public String Script { get; set; }
        public List<String> AvailableFields { get; set; }
        public String ModelName { get; set; }

        public override string ToString()
        {
            return Id + "," + ProjectId + "," + ModelId + "," + Type + "," + FieldId + "," + UseScript + "," + Script;
        }
    }
}
