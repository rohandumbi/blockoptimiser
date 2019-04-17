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
    class SchedulerQueueDataAccess : BaseDataAccess
    {
        public List<SchedulerQueue> GetAll()
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<SchedulerQueue>("select * from SchedulerQueue").ToList();
            }
        }

        public SchedulerQueue Get(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                String selectQuery = $"select * from SchedulerQueue where Id = { Id }";
                return connection.QuerySingle<SchedulerQueue>(selectQuery);
            }
        }

        public void Insert(SchedulerQueue newQueueItem)
        {

            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into SchedulerQueue (ProjectId, FileName, Year, Period, IsProcessed, UpdatedAt)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @FileName, @Year, @Period, 0, GETDATE())";
                newQueueItem.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newQueueItem.ProjectId,
                    newQueueItem.FileName,
                    newQueueItem.Year,
                    newQueueItem.Period
                });
            }
        }

        public void Delete(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                String deleteQuery = $"delete from SchedulerQueue where Id = { Id }";
                connection.Execute(deleteQuery);
            }
        }
    }
}
