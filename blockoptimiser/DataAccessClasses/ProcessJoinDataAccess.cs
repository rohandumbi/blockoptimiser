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
        public List<ProcessJoin> GetAll(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<ProcessJoin>($"select * from processjoin where ProjectId = { ProjectId } ").ToList();
            }
        }

        public void Insert(ProcessJoin newProcessJoin)
        {
            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into ProcessJoin (ProjectId, Name, FieldId, FilterString)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @Name, @FieldId, @FilterString)";

                connection.QuerySingle<int>(insertQuery, new
                {
                    newProcessJoin.ProjectId,
                    newProcessJoin.Name,
                    newProcessJoin.ChildProcessId
                });

            }
        }


        public void Delete(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                connection.Execute($"delete from ProcessJoin where Id = { Id }");
            }
        }
    }
}
