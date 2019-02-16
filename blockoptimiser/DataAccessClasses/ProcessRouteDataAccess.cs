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
    public class ProcessRouteDataAccess : BaseDataAccess
    {
        public List<ProcessRoute> GetAll(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<ProcessRoute>($"select * from processroute where ProjectId= { ProjectId } ").ToList();
            }
        }
        public void Insert(ProcessRoute newProcessRoute)
        {
            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into processroute (ProjectId, ProcessId, ParentProcessId)" +
                    $" VALUES(@ProjectId, @ProcessId, @ParentProcessId)";
                connection.Execute(insertQuery, new
                {
                    newProcessRoute.ProjectId,
                    newProcessRoute.ProcessId,
                    newProcessRoute.ParentProcessId
                });
            }
        }
        public void Delete(int ProjectId, int ProcessId)
        {
            using (IDbConnection connection = getConnection())
            {
                String deleteQuery = $"delete from processroute where ProjectId = { ProjectId } AND ProcessId = { ProcessId } ";
                connection.Execute(deleteQuery);
            }
        }
    }
}
