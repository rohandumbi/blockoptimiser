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
    public class BenchLimitDataAccess : BaseDataAccess
    {
        public List<BenchLimit> GetAll(int ScenarioId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<BenchLimit>($"select * from BenchLimit where ScenarioID = { ScenarioId }").ToList();
            }
        }

        public void Update(BenchLimit updatedBenchLimit)
        {
            using (IDbConnection connection = getConnection())
            {
                String updateQuery = $"update BenchLimit set ScenarioId = @ScenarioId, ModelId = @ModelId, ModelName = @ModelName, Value = @Value, IsUsed = @IsUsed  where Id = @Id ";
                connection.Execute(updateQuery, new
                {
                    updatedBenchLimit.ScenarioId,
                    updatedBenchLimit.ModelId,
                    updatedBenchLimit.ModelName,
                    updatedBenchLimit.Value,
                    updatedBenchLimit.IsUsed,
                    updatedBenchLimit.Id
                });
            }
        }

        public void Insert(BenchLimit newBenchLimit)
        {

            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into BenchLimit (ScenarioId, ModelId, ModelName,  Value, IsUsed)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ScenarioId, @ModelId, @ModelName, @Value, @IsUsed)";

                newBenchLimit.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newBenchLimit.ScenarioId,
                    newBenchLimit.ModelId,
                    newBenchLimit.ModelName,
                    newBenchLimit.Value,
                    newBenchLimit.IsUsed
                });
            }
        }
    }
}
