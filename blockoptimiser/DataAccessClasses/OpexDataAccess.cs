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
    public class OpexDataAccess : BaseDataAccess
    {
        public List<Opex> GetAll(int ScenarioId)
        {
            using (IDbConnection connection = getConnection())
            {
                var OpexList = connection.Query<Opex>($"select * from Opex where ScenarioID = { ScenarioId }").ToList();
                
                foreach (var opex in OpexList)
                {
                    opex.CostData
                        = connection.Query<OpexYearMapping>($"select * from OpexYearMapping where OpexId = { opex.Id }").ToList();
                }
                return OpexList;
            }
        }

        public void Insert(Opex newOpex)
        {

            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into Opex (ScenarioId, CostType, FilterType, FilterName, UnitType, UnitId, IsUsed)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ScenarioId, @CostType, @FilterType, @FilterName, @UnitType, @UnitId, @IsUsed)";

                String insertMappingQuery = $"insert into OpexYearMapping (OpexId, Year, Value)" +
                    $" VALUES(@OpexId, @Year, @Value)";

                newOpex.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newOpex.ScenarioId,
                    newOpex.CostType,
                    newOpex.FilterType,
                    newOpex.FilterName,
                    newOpex.UnitType,
                    newOpex.UnitId,
                    newOpex.IsUsed
                });

                foreach (OpexYearMapping opexYearMapping in newOpex.CostData)
                {
                    opexYearMapping.OpexId = newOpex.Id;
                    connection.Query(insertMappingQuery, new
                    {
                        opexYearMapping.OpexId,
                        opexYearMapping.Year,
                        opexYearMapping.Value
                    });
                }
            }
        }

        public void InsertOpexMapping(Opex opex)
        {

            using (IDbConnection connection = getConnection())
            {

                String insertMappingQuery = $"insert into OpexYearMapping (OpexId, Year, Value)" +
                    $" VALUES(@OpexId, @Year, @Value)";

                foreach (OpexYearMapping opexYearMapping in opex.CostData)
                {
                    opexYearMapping.OpexId = opex.Id;
                    connection.Query(insertMappingQuery, new
                    {
                        opexYearMapping.OpexId,
                        opexYearMapping.Year,
                        opexYearMapping.Value
                    });
                }
            }
        }

        public void DeleteOpexMapping(int OpexId)
        {

            using (IDbConnection connection = getConnection())
            {
                String deleteMappingQuery = $"delete from OpexYearMapping where OpexId = { OpexId } ";
                connection.Execute(deleteMappingQuery);
            }
        }
    }
}
