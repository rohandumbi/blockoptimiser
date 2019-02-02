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
                String insertQuery = $"insert into Geotech (ProjectId, ModelId, Type, FieldId, UseScript, Script)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @ModelId, @Type, @FieldId, @UseScript, @Script)";
                newGeotech.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newGeotech.ProjectId,
                    newGeotech.ModelId,
                    newGeotech.Type,
                    newGeotech.FieldId,
                    newGeotech.UseScript,
                    newGeotech.Script
                });
            }
        }

        public void Update(Geotech updatedGeoTech)
        {
            using (IDbConnection connection = getConnection())
            {
                String updateQuery = $"update Geotech set ProjectId = @ProjectId, ModelId = @ModelId, Type = @Type, FieldId = @FieldId, UseScript = @UseScript, Script = @Script where Id = @Id ";
                connection.Execute(updateQuery, new
                {
                    updatedGeoTech.ProjectId,
                    updatedGeoTech.ModelId,
                    updatedGeoTech.Type,
                    updatedGeoTech.FieldId,
                    updatedGeoTech.UseScript,
                    updatedGeoTech.Script,
                    updatedGeoTech.Id
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
