using blockoptimiser.DataAccessClasses;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Services.DataImport
{
    public class CSVDataLoader : BaseDataAccess
    {
        private CSVReader reader;
        public CSVDataLoader(CSVReader rdr)
        {
            reader = rdr;
        }
        public void Load()
        {
            DropTable();
            CreateTable();
            System.Data.SqlClient.SqlBulkCopy bcp =
                new SqlBulkCopy(connectionString, SqlBulkCopyOptions.UseInternalTransaction);
            bcp.BatchSize = 500;
            bcp.DestinationTableName = "BOData_" + Context.ProjectId + "_" + Context.ModelId;
            bcp.NotifyAfter = 500;
            bcp.SqlRowsCopied += (sender, e) =>
            {
                Console.WriteLine("Written: " + e.RowsCopied.ToString());
            };
            bcp.WriteToServer(reader);
        }

        private void DropTable()
        {
            using (IDbConnection connection = getConnection())
            {
                String ddl = "drop table BOData_" + Context.ProjectId + "_" + Context.ModelId;
                try
                {
                    connection.Execute(ddl);
                } catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                
            }
        }

        private void CreateTable()
        {
            using (IDbConnection connection = getConnection())
            {
                String ddl = "create table BOData_" + Context.ProjectId + "_" + Context.ModelId + " ( ";
                for (int i = 0; i < reader.Header.Length; i++)
                {
                    if (i == 0)
                    {
                        ddl += $" { reader.Header[i]} VARCHAR(100) ";
                    }
                    else
                    {
                        ddl += $", { reader.Header[i]} VARCHAR(100) ";
                    }

                }
                ddl += " ) ";
                connection.Execute(ddl);
            }
        }
    }
}
