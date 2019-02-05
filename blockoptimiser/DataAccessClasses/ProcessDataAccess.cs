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
    public class ProcessDataAccess : BaseDataAccess
    {
        public List<Process> GetAll(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<Process>($"select * from process where ProjectId = { ProjectId } ").ToList();
            }
        }

        public void Insert(Process newProcess)
        {
            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into Process (ProjectId, Name, FieldId, FilterString)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @Name, @FieldId, @FilterString)";

                newProcess.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newProcess.ProjectId,
                    newProcess.Name,
                    newProcess.FieldId,
                    newProcess.FilterString
                });

            }
        }

        public void Delete(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                connection.Execute($"delete from Process where Id = { Id }");
            }
        }
    }
}
