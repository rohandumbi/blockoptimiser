using blockoptimiser.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.DataAccessClasses
{
    public class SchedulerResultDataAccess : BaseDataAccess
    {
        public void Create(int ProjectId)
        {
            Drop(); // Drop first
            using (IDbConnection connection = getConnection())
            {
                String create_sql = "create table BOResult_" + Context.ProjectId + " ( " +
                    " BId BIGINT, " +
                    " DestinationType TINYINT, " +
                    " Destination INT," +
                    " Year INT," +
                    " MinedYear INT," +
                    " AdjustedYear INT," +
                    " TonnageMined DECIMAL(18,10) )";
                
                try
                {
                    connection.Execute(create_sql);
                } catch(Exception e)
                {
                    Console.WriteLine("Error while creating result table :" + e.Message);
                }
                
            }
        }

        public Dictionary<int, Dictionary<int, Dictionary<int, MinedBlock>>> GetMinedBlocks(int ProjectId, int ModelId, int year)
        {
            Dictionary<int, Dictionary<int, Dictionary<int, MinedBlock>>> blocks = new Dictionary<int, Dictionary<int, Dictionary<int, MinedBlock>>>();
            using (IDbConnection connection = getConnection())
            {
                try
                {
                    List<MinedBlock> minedBlocks =  connection.Query<MinedBlock>($"select distinct a.bid, a.year, i, j, k from BOResult_{ ProjectId} a, BOData_Computed_{ ProjectId }_{ ModelId } b" +
                        $" where a.bid = b.bid and a.year = { year } ").ToList();


                    foreach(MinedBlock minedBlock in minedBlocks)
                    {
                        if (!blocks.ContainsKey(minedBlock.I))
                        {
                            Dictionary<int, MinedBlock> zblocks = new Dictionary<int, MinedBlock>();
                            Dictionary<int, Dictionary<int, MinedBlock>> yblocks = new Dictionary<int, Dictionary<int, MinedBlock>>();
                            zblocks.Add(minedBlock.K, minedBlock);
                            yblocks.Add(minedBlock.J, zblocks);
                            blocks.Add(minedBlock.I, yblocks);
                        }
                        else
                        {
                            Dictionary<int, Dictionary<int, MinedBlock>> yblocks = blocks[minedBlock.I];
                            if (!yblocks.ContainsKey(minedBlock.J))
                            {
                                Dictionary<int, MinedBlock> zblocks = new Dictionary<int, MinedBlock>();
                                zblocks.Add(minedBlock.K, minedBlock);
                                yblocks.Add(minedBlock.J, zblocks);
                            }
                            else
                            {
                                Dictionary<int, MinedBlock> zblocks = yblocks[minedBlock.J];
                                if (!zblocks.ContainsKey(minedBlock.K))
                                {
                                    zblocks.Add(minedBlock.K, minedBlock);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while accessing result table : " + e.Message);
                }
            }
            return blocks;
        }
        public void UpdateYear(int ProjectId, List<MinedBlock> minedBlocks)
        {
            using (IDbConnection connection = getConnection())
            {
                foreach(MinedBlock minedBlock in minedBlocks)
                {
                    try
                    {
                        connection.Execute($"update BOResult_{ ProjectId } set year = { minedBlock.Year }, adjustedYear = { minedBlock.Year }  where bid = { minedBlock.Bid } ");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error while updating result table : " + e.Message);
                    }
                }

            }
        }
        private void Drop()
        {
            using (IDbConnection connection = getConnection())
            {
                String ddl = "drop table BOResult_" + Context.ProjectId;
                try
                {
                    connection.Execute(ddl);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while dropping result table : "+e.Message);
                }
           }
        }
    }
}
