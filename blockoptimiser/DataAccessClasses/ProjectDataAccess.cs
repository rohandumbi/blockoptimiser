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
    public class ProjectDataAccess : BaseDataAccess
    {
        public List<ProjectModel> GetAll()
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<ProjectModel>("select * from project").ToList();
            }
        }

        public void Insert(ProjectModel newProject)
        {

            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into Project (Name, Description, CreatedDate, ModifiedDate)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@Name, @Description, GETDATE(), GETDATE())";
                newProject.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newProject.Name,
                    newProject.Description
                });
            }
        }

        public void Delete(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                String deleteQuery = $"delete from Project where Id = { ProjectId }";
                connection.Execute(deleteQuery);
            }
        }
    }
}
