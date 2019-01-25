using blockoptimiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.DataAccessClasses
{
    public class ExpressionDataAccess : BaseDataAccess
    {
        public List<Expression> GetAll(int ProjectId)
        {
            return new List<Expression> {
                new Expression(1, 1, "model 1", "bin>30"),
                new Expression(2, 1, "model 1", "bin=30"),
                new Expression(3, 2, "model 2", "exp>30"),
                new Expression(4, 2, "model 2", "exp=30")
            };
            
        }
    }
}
