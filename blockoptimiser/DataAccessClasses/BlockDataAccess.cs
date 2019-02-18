using blockoptimiser.Models;
using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.DataAccessClasses
{
    public class BlockDataAccess : BaseDataAccess
    {
        public List<Block> GetBlocks(int ProjectId, int ModelId, String condition)
        {
            using (IDbConnection connection = getConnection())
            {
                List<Block> blocks = new List<Block>();
                 List<object> rows = connection.Query($"select * from BOData_{ ProjectId }_{ ModelId } where { condition } ").ToList();
                foreach(Object row in rows)
                {
                    IDictionary<string, object> rowDictionary = (IDictionary<string, object>)row;
                    Block block = new Block
                    {
                        Id = (int)rowDictionary["Id"],
                        data = rowDictionary
                    };
                    blocks.Add(block);
                    
                }
                return blocks;
            }
        }
        public String GetAngle(int ProjectId, int ModelId, String selectstr)
        {
            using (IDbConnection connection = getConnection())
            {             
                return connection.QuerySingle<String>($"select { selectstr} from BOData_{ ProjectId }_{ ModelId } ");
                
            }
        }
    }
}
