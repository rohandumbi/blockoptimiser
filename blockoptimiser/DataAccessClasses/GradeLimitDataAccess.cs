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
    public class GradeLimitDataAccess : BaseDataAccess
    {
        public List<GradeLimit> GetGradeLimits()
        {
            using (IDbConnection connection = getConnection())
            {
                List<GradeLimit> GradeLimits = connection.Query<GradeLimit>($"select * from GradeLimit where ScenarioID = { Context.ScenarioId }").ToList();
                
                foreach (var GradeLimit in GradeLimits)
                {
                    GradeLimit.GradeLimitYearMapping
                        = connection.Query<GradeLimitYearMapping>($"select * from GradeLimitYearMapping where GradeLimitId = { GradeLimit.Id }").ToList();
                }
                return GradeLimits;
            }
        }
        public void Update(GradeLimit updatedGradeLimit)
        {
            using (IDbConnection connection = getConnection())
            {
                String updateQuery = $"update GradeLimit set ScenarioId = @ScenarioId, IsMax = @IsMax, ItemName = @ItemName, ItemId = @ItemId, ItemType = @ItemType, IsUsed = @IsUsed, GradeName = @GradeName  where Id = @Id ";
                connection.Execute(updateQuery, new
                {
                    updatedGradeLimit.ScenarioId,
                    updatedGradeLimit.IsMax,
                    updatedGradeLimit.ItemName,
                    updatedGradeLimit.ItemId,
                    updatedGradeLimit.ItemType,
                    updatedGradeLimit.IsUsed,
                    updatedGradeLimit.Id,
                    updatedGradeLimit.GradeName
                });
            }
        }

        public void Insert(GradeLimit newGradeLimit)
        {

            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into GradeLimit (ScenarioId, IsMax, ItemName, ItemId, ItemType, IsUsed, GradeName)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ScenarioId, @IsMax, @ItemName, @ItemId, @ItemType, @IsUsed, @GradeName)";

                String insertMappingQuery = $"insert into GradeLimitYearMapping (GradeLimitId, Year, Value)" +
                    $" VALUES(@GradeLimitId, @Year, @Value)";

                newGradeLimit.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newGradeLimit.ScenarioId,
                    newGradeLimit.IsMax,
                    newGradeLimit.ItemName,
                    newGradeLimit.ItemId,
                    newGradeLimit.ItemType,
                    newGradeLimit.IsUsed,
                    newGradeLimit.GradeName
                });

                foreach (GradeLimitYearMapping GradeLimitYearMapping in newGradeLimit.GradeLimitYearMapping)
                {
                    GradeLimitYearMapping.GradeLimitId = newGradeLimit.Id;
                    connection.Query(insertMappingQuery, new
                    {
                        GradeLimitYearMapping.GradeLimitId,
                        GradeLimitYearMapping.Year,
                        GradeLimitYearMapping.Value
                    });
                }
            }
        }

        public void InsertGradeLimitMapping(GradeLimit gradeLimit)
        {

            using (IDbConnection connection = getConnection())
            {

                String insertMappingQuery = $"insert into GradeLimitYearMapping (GradeLimitId, Year, Value)" +
                    $" VALUES(@GradeLimitId, @Year, @Value)";

                foreach (GradeLimitYearMapping GradeLimitYearMapping in gradeLimit.GradeLimitYearMapping)
                {
                    GradeLimitYearMapping.GradeLimitId = gradeLimit.Id;
                    connection.Query(insertMappingQuery, new
                    {
                        GradeLimitId = GradeLimitYearMapping.GradeLimitId,
                        GradeLimitYearMapping.Year,
                        GradeLimitYearMapping.Value
                    });
                }
            }
        }

        public void DeleteGradeLimitMapping(int GradeLimitId)
        {

            using (IDbConnection connection = getConnection())
            {

                String deleteMappingQuery = $"delete from GradeLimitYearMapping where GradeLimitId = {GradeLimitId} ";
                connection.Execute(deleteMappingQuery);
            }
        }
    }
}
