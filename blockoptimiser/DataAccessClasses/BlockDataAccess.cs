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
                 List<object> rows = connection.Query($"select a.* , b.bid as bid from " +
                     $"BOData_{ ProjectId }_{ ModelId } a, BOData_Computed_{ ProjectId }_{ ModelId } b where a.id = b.id and { condition } ").ToList();
                foreach(Object row in rows)
                {
                    IDictionary<string, object> rowDictionary = (IDictionary<string, object>)row;
                    Block block = new Block
                    {
                        Id = (long)rowDictionary["bid"],
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

        public List<Block> GetBlocksByFields(int ProjectId, int ModelId, List<String> columns)
        {
            using (IDbConnection connection = getConnection())
            {
                List<Block> blocks = new List<Block>();
                String sql = $"select bid, i, j, k,xortho, yortho, zortho " ;
                foreach(String column in columns)
                {
                    sql = sql + ","+ column;
                }
                sql = sql + $" from BOData_{ ProjectId }_{ ModelId } a, BOData_Computed_{ ProjectId }_{ ModelId } b where a.id = b.id ";
                List<object> rows = connection.Query(sql).ToList();
                foreach (Object row in rows)
                {
                    IDictionary<string, object> rowDictionary = (IDictionary<string, object>)row;
                    Block block = new Block
                    {
                        Id = (long)rowDictionary["bid"],
                        data = rowDictionary
                    };
                    blocks.Add(block);

                }
                return blocks;
            }
        }
    }
}
