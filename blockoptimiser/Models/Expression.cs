using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class Expression
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public String Name { get; set; }
        public List<ExprModelMapping> modelMapping { get; set; }
    }

    public class ExprModelMapping
    {
        public int ExprId { get; set; }
        public int ModelId { get; set; }
        public String ModelName { get; set; }
        public String ExprString { get; set; }
    }
}
