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
        public int ModelId { get; set; }
        public String ModelName { get; set; }
        public String ExpressionString { get; set; }

        public Expression(int id, int modelId, String modelName, String expression)
        {
            Id = id;
            ModelId = modelId;
            ModelName = modelName;
            ExpressionString = expression;
        }
    }
}
