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
    public class ProcessLimitDataAccess : BaseDataAccess
    {
        public List<ProcessLimit> GetAll(int ScenarioId)
        {
            using (IDbConnection connection = getConnection())
            {
                var ProcessLimits = connection.Query<ProcessLimit>($"select * from ProcessLimit where ScenarioID = { ScenarioId }").ToList();
                
                foreach (var ProcessLimit in ProcessLimits)
                {
                    ProcessLimit.ProcessLimitYearMapping
                        = connection.Query<ProcessLimitYearMapping>($"select * from ProcessLimitYearMapping where ProcessLimitId = { ProcessLimit.Id }").ToList();
                }
                return ProcessLimits;
            }
        }

        public void Update(ProcessLimit updatedProcessLimit)
        {
            using (IDbConnection connection = getConnection())
            {
                String updateQuery = $"update ProcessLimit set ScenarioId = @ScenarioId, ItemName = @ItemName, ItemId = @ItemId, ItemType = @ItemType, IsUsed = @IsUsed  where Id = @Id ";
                connection.Execute(updateQuery, new
                {
                    updatedProcessLimit.ScenarioId,
                    updatedProcessLimit.ItemName,
                    updatedProcessLimit.ItemId,
                    updatedProcessLimit.ItemType,
                    updatedProcessLimit.IsUsed,
                    updatedProcessLimit.Id
                });
            }
        }

        public void InsertProcessLimit(ProcessLimit newProcessLimit)
        {

            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into ProcessLimit (ScenarioId, ItemName, ItemId, ItemType, IsUsed)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ScenarioId, @ItemName, @ItemId, @ItemType, @IsUsed)";

                String insertMappingQuery = $"insert into ProcessLimitYearMapping (ProcessLimitId, Year, Value)" +
                    $" VALUES(@ProcessLimitId, @Year, @Value)";

                newProcessLimit.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newProcessLimit.ScenarioId,
                    newProcessLimit.ItemName,
                    newProcessLimit.ItemId,
                    newProcessLimit.ItemType,
                    newProcessLimit.IsUsed
                });

                foreach (ProcessLimitYearMapping ProcessLimitYearMapping in newProcessLimit.ProcessLimitYearMapping)
                {
                    ProcessLimitYearMapping.ProcessLimitId = newProcessLimit.Id;
                    connection.Query(insertMappingQuery, new
                    {
                        ProcessLimitYearMapping.ProcessLimitId,
                        ProcessLimitYearMapping.Year,
                        ProcessLimitYearMapping.Value
                    });
                }
            }
        }

        public void InsertProcessLimitMapping(ProcessLimit ProcessLimit)
        {

            using (IDbConnection connection = getConnection())
            {

                String insertMappingQuery = $"insert into ProcessLimitYearMapping (ProcessLimitId, Year, Value)" +
                    $" VALUES(@ProcessLimitId, @Year, @Value)";

                foreach (ProcessLimitYearMapping ProcessLimitYearMapping in ProcessLimit.ProcessLimitYearMapping)
                {
                    ProcessLimitYearMapping.ProcessLimitId = ProcessLimit.Id;
                    connection.Query(insertMappingQuery, new
                    {
                        ProcessLimitYearMapping.ProcessLimitId,
                        ProcessLimitYearMapping.Year,
                        ProcessLimitYearMapping.Value
                    });
                }
            }
        }

        public void DeleteProcessLimitMapping(int ProcessLimitId)
        {

            using (IDbConnection connection = getConnection())
            {

                String deleteMappingQuery = $"delete from ProcessLimitYearMapping where ProcessLimitId = {ProcessLimitId} ";
                connection.Execute(deleteMappingQuery);
            }
        }
    }
}
