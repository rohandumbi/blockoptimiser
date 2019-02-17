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
        public List<ProcessLimitModel> GetProcessLimits()
        {
            using (IDbConnection connection = getConnection())
            {
                var ProcessLimits = connection.Query<ProcessLimitModel>($"select * " +
                    $"from ProcessLimit " +
                    $"where ScenarioID = { Context.ScenarioId }").ToList();
                
                foreach (var ProcessLimit in ProcessLimits)
                {
                    ProcessLimit.ProcessLimitYearMapping
                        = connection.Query<ProcessLimitYearMappingModel>($"select * from ProcessLimitYearMapping where ProcessLimitId = { ProcessLimit.Id }").ToList();
                }
                return ProcessLimits;
            }
        }

        public void InsertProcessLimit(ProcessLimitModel newProcessLimit)
        {

            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into ProcessLimit (ScenarioId, ItemName, ItemId, ItemType)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ScenarioId, @ItemName, @ItemId, @ItemType)";

                String insertMappingQuery = $"insert into ProcessLimitYearMapping (ProcessLimitId, Year, Value)" +
                    $" VALUES(@ProcessLimitId, @Year, @Value)";

                newProcessLimit.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newProcessLimit.ScenarioId,
                    newProcessLimit.ItemName,
                    newProcessLimit.ItemId,
                    newProcessLimit.ItemType
                });

                foreach (ProcessLimitYearMappingModel ProcessLimitYearMapping in newProcessLimit.ProcessLimitYearMapping)
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

        public void InsertProcessLimitMapping(ProcessLimitModel ProcessLimit)
        {

            using (IDbConnection connection = getConnection())
            {

                String insertMappingQuery = $"insert into ProcessLimitYearMapping (ProcessLimitId, Year, Value)" +
                    $" VALUES(@ProcessLimitId, @Year, @Value)";

                foreach (ProcessLimitYearMappingModel ProcessLimitYearMapping in ProcessLimit.ProcessLimitYearMapping)
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
