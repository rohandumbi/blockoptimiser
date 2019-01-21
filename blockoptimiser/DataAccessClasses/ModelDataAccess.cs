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
    public class ModelDataAccess : BaseDataAccess
    {
        public List<Model> GetAll()
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<Model>("select * from model").ToList();
            }
        }

        public List<Model> GetAll(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<Model>("select * from model where ProjectId=" + ProjectId).ToList();
            }
        }

        public void Insert(Model newModel)
        {

            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into model (ProjectId, Name)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @Name)";
                newModel.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newModel.ProjectId,
                    newModel.Name
                });
            }
        }

        public void Delete(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                String deleteQuery = $"delete from model where Id = { Id }";
                connection.Execute(deleteQuery);
            }
        }
    }
}
