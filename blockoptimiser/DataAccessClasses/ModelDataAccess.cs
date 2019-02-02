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

        public List<Model> GetAll(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<Model>($"select * from model where ProjectId= { ProjectId } ").ToList();
            }
        }

        public Model Get(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.QuerySingle<Model>($"select * from model where Id= { Id } ");
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

        public void Update(Model model)
        {

            using (IDbConnection connection = getConnection())
            {
                String updateQuery = $"update model set Bearing = @Bearing, HasData = @HasData where Id = @Id ";
                connection.Execute(updateQuery, new
                {
                    model.Bearing,
                    model.HasData,
                    model.Id
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
