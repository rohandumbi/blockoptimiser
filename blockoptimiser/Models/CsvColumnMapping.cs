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
        public string DefaultValue { get; set; }
        public int FieldId { get; set; }
        public int DataType { get; set; }
        public int AssociatedField { get; set; }

        public override string ToString()
        {
            return Id + "," + ModelId + "," + ColumnName + "," + FieldId + "," + DefaultValue;
        }
    }
}
