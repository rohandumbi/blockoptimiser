using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class RequiredFieldMapping
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public String RequiredFieldName { get; set; }
        public int FieldId { get; set; }
    }
}
