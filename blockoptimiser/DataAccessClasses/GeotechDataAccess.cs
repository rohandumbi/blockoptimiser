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
        public List<Geotech> GetAll(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<Geotech>($"select * from geotech where ProjectId  = { ProjectId }").ToList();
            }
        }
        public Geotech Get(int ModelId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.QuerySingle<Geotech>("select * from geotech where ModelId =" + ModelId);
            }
        }

        public void Insert(Geotech newGeotech)
        {

            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into Geotech (ProjectId, ModelId, Type, FieldId, Script)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @ModelId, @Type, @FieldId, @Script)";
                newGeotech.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newGeotech.ProjectId,
                    newGeotech.ModelId,
                    newGeotech.Type,
                    newGeotech.FieldId,
                    newGeotech.Script
                });
            }
        }

        public void Delete(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                String deleteQuery = $"delete from geotech where Id = { Id }";
                connection.Execute(deleteQuery);
            }
        }
    }
}
