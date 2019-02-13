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
    public class ProcessJoinDataAccess : BaseDataAccess
    {
        public List<String> GetProcessJoins(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<String>($"select distinct Name from processjoin where ProjectId = { ProjectId } ").ToList();
            }
        }

        public List<String> GetProcessesInJoin(String ProcessJoinName)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<String>($"select b.name from processjoin a, process b " +
                    $"where a.processid = b.id  and a.name = '{ ProcessJoinName }' ").ToList();
            }
        }

        public void Insert(ProcessJoin newProcessJoin)
        {
            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into ProcessJoin (ProjectId, Name, FieldId, FilterString)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @Name, @FieldId, @FilterString)";

                foreach(int ProcessId in newProcessJoin.ProcessIds)
                {
                    connection.QuerySingle<int>(insertQuery, new
                    {
                        newProcessJoin.ProjectId,
                        newProcessJoin.Name,
                        ProcessId
                    });
                }           
            }
        }


        public void Delete(String ProcessJoinName)
        {
            using (IDbConnection connection = getConnection())
            {
                connection.Execute($"delete from ProcessJoin where name = '{ ProcessJoinName }'");
            }
        }

        public void DeleteProcess(String ProcessJoinName, int ProcessId)
        {
            using (IDbConnection connection = getConnection())
            {
                connection.Execute($"delete from ProcessJoin where name = '{ ProcessJoinName }' and processId = { ProcessId } ");
            }
        }
    }
}
