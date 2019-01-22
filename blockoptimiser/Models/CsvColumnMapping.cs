using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class CsvColumnMapping
    {
        public int Id { get; set; }
        public int ModelId { get; set; }
        public String ColumnName { get; set; }
        public int FieldId { get; set; }
    }
}
