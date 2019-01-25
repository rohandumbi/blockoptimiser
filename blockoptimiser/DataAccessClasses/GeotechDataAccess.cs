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
    public class GeotechDataAccess : BaseDataAccess
    {
    
        public Geotech Get(int ModelId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.QuerySingle<Geotech>("select * from field geotech ModelId =" + ModelId);
            }
        }

        public void Insert(Field newField)
        {

            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into field (ProjectId, Name, DataType, WeightedUnit)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @Name, @DataType, @WeightedUnit)";
                newField.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newField.ProjectId,
                    newField.Name,
                    newField.DataType,
                    newField.WeightedUnit
                });
            }
        }

        public void Delete(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                String deleteQuery = $"delete from field where Id = { Id }";
                connection.Execute(deleteQuery);
            }
        }
    }
}
