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
    public class ProjectDataDataAccess : BaseDataAccess
    {
        public List<ProjectDataModel> GetAll()
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<ProjectDataModel>("select * from ProjectData").ToList();
            }
        }

        public List<ProjectDataModel> GetAll(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<ProjectDataModel>("select * from ProjectData where ProjectId=" + ProjectId).ToList();
            }
        }

        public void Insert(ProjectDataModel newProjectData)
        {

            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into ProjectData (ProjectId, Name, Bearing)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @Name, @Bearing)";
                newProjectData.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newProjectData.ProjectId,
                    newProjectData.Name,
                    newProjectData.Bearing
                });
            }
        }

        public void Delete(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                String deleteQuery = $"delete from ProjectData where Id = { Id }";
                connection.Execute(deleteQuery);
            }
        }
    }
}
