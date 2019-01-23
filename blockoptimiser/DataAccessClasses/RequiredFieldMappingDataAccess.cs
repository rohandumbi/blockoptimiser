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
        public List<RequiredFieldMapping> GetAll()
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<RequiredFieldMapping>("select * from RequiredFieldMapping").ToList();
            }
        }

        public List<RequiredFieldMapping> GetAll(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<RequiredFieldMapping>("select * from RequiredFieldMapping where ProjectId=" + ProjectId).ToList();
            }
        }

        public void Insert(RequiredFieldMapping newRequiredFieldMapping)
        {

            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into RequiredFieldMapping (ProjectId, RequiredFieldName, FieldId)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @RequiredFieldName, @FieldId)";
                newRequiredFieldMapping.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newRequiredFieldMapping.ProjectId,
                    newRequiredFieldMapping.RequiredFieldName,
                    newRequiredFieldMapping.FieldId
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
