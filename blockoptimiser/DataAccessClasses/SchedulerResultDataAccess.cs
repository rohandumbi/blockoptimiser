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

        public List<long> GetMinedBlocks(int ProjectId)
        {
            List<long> minedBlocks = new List<long>();
            using (IDbConnection connection = getConnection())
            {
                try
                {
                    minedBlocks = connection.Query<long>("select distinct BId from BOResult_"+ Context.ProjectId).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while dropping result table : " + e.Message);
                }
            }
            return minedBlocks;
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
