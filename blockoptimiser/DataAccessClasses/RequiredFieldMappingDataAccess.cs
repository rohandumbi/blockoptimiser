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
    public class RequiredFieldMappingDataAccess : BaseDataAccess
    {
        
        public List<RequiredFieldMapping> GetAll(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<RequiredFieldMapping>($"select * from RequiredFieldMapping where ProjectId= { ProjectId } ").ToList();
            }
        }

        public void Insert(RequiredFieldMapping newRequiredFieldMapping)
        {

            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into RequiredFieldMapping (ProjectId, RequiredFieldName, MappedColumnName)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @RequiredFieldName, @MappedColumnName)";
                newRequiredFieldMapping.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newRequiredFieldMapping.ProjectId,
                    newRequiredFieldMapping.RequiredFieldName,
                    newRequiredFieldMapping.MappedColumnName
                });
            }
        }

        public void Update(RequiredFieldMapping requiredFieldMapping)
        {

            using (IDbConnection connection = getConnection())
            {
                String updateQuery = $"update RequiredFieldMapping set MappedColumnName  =  @MappedColumnName where Id = @Id ";
                connection.Execute(updateQuery, new
                {
                    requiredFieldMapping.MappedColumnName,
                    requiredFieldMapping.Id
                });
            }
        }

        public void Delete(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                String deleteQuery = $"delete from RequiredFieldMapping where Id = { Id }";
                connection.Execute(deleteQuery);
            }
        }
    }
}
