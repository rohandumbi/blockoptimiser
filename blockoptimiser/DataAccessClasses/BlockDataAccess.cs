using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.DataAccessClasses
{
    public class BlockDataAccess : BaseDataAccess
    {
        public List<Object> GetBlocks(int ProjectId, int ModelId, String condition)
        {
            using (IDbConnection connection = getConnection())
            {
                List<object> result = connection.Query($"select * from BOData_{ ProjectId }_{ ModelId } where { condition } ").ToList();
                return result;
            }
        }
    }
}
