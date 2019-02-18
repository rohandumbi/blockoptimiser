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
    public class ScenarioDataAccess : BaseDataAccess
    {
        public List<Scenario> GetAll()
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<Scenario>($"select * from Scenario where ProjectId = { Context.ProjectId } ").ToList();
            }
        }

        public Scenario Get(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.QuerySingle<Scenario>($"select * from Scenario where Id = { Id } ");
            }
        }

        public void Insert(Scenario newScenario)
        {
            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into Scenario (ProjectID, Name, StartYear, TimePeriod, DiscountFactor ) " +
                    $" OUTPUT INSERTED.Id  " +
                    $" values ( @ProjectId, @Name, @StartYear, @TimePeriod, @DiscountFactor)";
                newScenario.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newScenario.ProjectId,
                    newScenario.Name,
                    newScenario.StartYear,
                    newScenario.TimePeriod,
                    newScenario.DiscountFactor
                });
            }

        }
    }
}
