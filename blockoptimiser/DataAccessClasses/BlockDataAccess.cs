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
                 List<object> rows = connection.Query($"select a.* , b.* from " +
                     $" BOData_{ ProjectId }_{ ModelId } a, BOData_Computed_{ ProjectId }_{ ModelId } b where a.id = b.id and { condition }" +
                     $" and b.Bid not in ( select distinct BId from BOResult_" + Context.ProjectId +")" ).ToList();
                foreach(Object row in rows)
                {
                    IDictionary<string, object> rowDictionary = (IDictionary<string, object>)row;
                    Block block = new Block
                    {
                        Id = (long)rowDictionary["Bid"],
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

        public Dictionary<int, Dictionary<int, Dictionary<int, Block>>> GetGeotechBlocks(int ProjectId, int ModelId, List<String> columns)
        {
            
            using (IDbConnection connection = getConnection())
            {
                Dictionary<int, Dictionary<int, Dictionary<int, Block>>> blocks = new Dictionary<int, Dictionary<int, Dictionary<int, Block>>>();
                String sql = $"select bid, i, j, k,xortho, yortho, zortho " ;
                foreach(String column in columns)
                {
                    sql = sql + ","+ column;
                }
                sql = sql + $" from BOData_{ ProjectId }_{ ModelId } a, BOData_Computed_{ ProjectId }_{ ModelId } b where a.id = b.id" +
                    $" and b.bid not in ( select distinct BId from BOResult_" + Context.ProjectId + ") order by i,j,k asc ";
                Console.WriteLine("Sql :>"+ sql);
                List<object> rows = connection.Query(sql).ToList();
                foreach (Object row in rows)
                {
                    IDictionary<string, object> rowDictionary = (IDictionary<string, object>)row;
                    Block block = new Block
                    {
                        Id = (long)rowDictionary["bid"],
                        data = rowDictionary
                    };
                    int i = (int)rowDictionary["i"];
                    int j = (int)rowDictionary["j"];
                    int k = (int)rowDictionary["k"];
                    if(!blocks.ContainsKey(i))
                    {
                        Dictionary<int, Block> zblocks = new Dictionary<int, Block>();
                        Dictionary<int, Dictionary<int, Block>> yblocks = new Dictionary<int, Dictionary<int, Block>>();
                        zblocks.Add(k, block);
                        yblocks.Add(j, zblocks);
                        blocks.Add(i, yblocks);
                    } else
                    {
                        Dictionary<int, Dictionary<int, Block>> yblocks = blocks[i];
                        if(!yblocks.ContainsKey(j))
                        {
                            Dictionary<int, Block> zblocks = new Dictionary<int, Block>();
                            zblocks.Add(k, block);
                            yblocks.Add(j, zblocks);
                        } else
                        {
                            Dictionary<int, Block> zblocks = yblocks[j];
                            if (!zblocks.ContainsKey(k))
                            {
                                zblocks.Add(k, block);
                            }
                        }
                    }

                }
                return blocks;
            }
        }
    }
}
