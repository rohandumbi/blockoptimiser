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
    public class ProcessDataAccess : BaseDataAccess
    {
        public List<Process> GetAll(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                List<Process> Processes =  connection.Query<Process>($"select ROW_NUMBER() OVER( ORDER BY Id asc ) AS 'ProcessNumber', * from process where ProjectId = { ProjectId } ").ToList();
                foreach(Process process in Processes)
                {
                    process.Mapping = connection.Query<ProcessModelMapping>($"select * from processmodelmapping where Processid = { process.Id } ").ToList();
                }
                return Processes;
            }

        }

        public void Insert(Process newProcess)
        {
            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into Process (ProjectId, Name)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @Name)";
                String insertMappingQuery = $"insert into ProcessModelMapping (ProcessId, ModelId, FilterString) " +
                    $" VALUES(@ProcessId, @ModelId, @FilterString)";

                newProcess.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newProcess.ProjectId,
                    newProcess.Name
                });

                foreach(ProcessModelMapping mapping in newProcess.Mapping)
                {
                    connection.Execute(insertMappingQuery, new
                    {
                        ProcessId = newProcess.Id,
                        mapping.ModelId,
                        mapping.FilterString
                    });
                }

            }
        }

        public void Delete(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                connection.Execute($"delete from ProcessModelMapping where ProcessId = { Id }");
                connection.Execute($"delete from Process where Id = { Id }");
            }
        }
    }
}
