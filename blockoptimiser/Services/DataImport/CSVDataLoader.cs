using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
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
                new SqlBulkCopy(connectionString, SqlBulkCopyOptions.UseInternalTransaction)
                {
                    BatchSize = 500,
                    DestinationTableName = "BOData_" + Context.ProjectId + "_" + Context.ModelId,
                    NotifyAfter = 500
                };
            bcp.SqlRowsCopied += (sender, e) =>
            {
                Console.WriteLine("Written: " + e.RowsCopied.ToString());
            };
            bcp.WriteToServer(reader);
        }

        public void LoadComputedDataTable(Dictionary<String, String> fixedFieldMapping, List<ModelDimension> modelDimensions, decimal angle)
        {
            decimal xinc = 0, yinc = 0, zinc = 0, xm = 0, ym = 0, zm = 0;
            foreach(ModelDimension dimension in modelDimensions)
            {
                if (dimension.Type.Equals("Origin"))
                {
                    xm = dimension.XDim;
                    ym = dimension.YDim;
                    zm = dimension.ZDim;
                } 
                else if (dimension.Type.Equals("Dimensions"))
                {
                    xinc = dimension.XDim;
                    yinc = dimension.YDim;
                    zinc = dimension.ZDim;
                }
            }
            Decimal cosAngleValue = new Decimal(Math.Cos((double)angle * Math.PI / 180));
            Decimal sinAngleValue = new Decimal(Math.Sin((double)angle * Math.PI / 180));
            new Thread(() =>
            {
                String sql = $"select Id, { fixedFieldMapping["x"]} as X, { fixedFieldMapping["y"]} as Y, { fixedFieldMapping["z"]} as Z, { fixedFieldMapping["tonnage"]} as Tonnage " +
                $" from BOData_{ Context.ProjectId }_{ Context.ModelId }";
                using (IDbConnection connection = getConnection())
                {
                    List<Row> rows = connection.Query<Row>(sql).AsList();

                    var dt = new DataTable();
                    dt.Columns.Add("Id");
                    dt.Columns.Add("Bid");
                    dt.Columns.Add("I");
                    dt.Columns.Add("J");
                    dt.Columns.Add("K");
                    dt.Columns.Add("Xortho");
                    dt.Columns.Add("Yortho");
                    dt.Columns.Add("Zortho");

                    foreach(Row row in rows)
                    {
                        row.Bid = long.Parse($"1{Context.ModelId.ToString("D5")}{row.Id}");
                        row.Xortho = cosAngleValue * Decimal.Parse(row.X) - sinAngleValue * Decimal.Parse(row.Y) + xm - cosAngleValue * xm + sinAngleValue * ym;
                        row.Yortho = sinAngleValue * Decimal.Parse(row.X) + cosAngleValue * Decimal.Parse(row.Y) + ym - sinAngleValue * xm - cosAngleValue * ym;
                        row.I = Decimal.ToInt32((row.Xortho + xinc - xm) / xinc);
                        row.J = Decimal.ToInt32((row.Yortho + yinc - ym) / yinc);
                        row.K = Decimal.ToInt32((row.Zortho + zinc - zm) / zinc);
                        dt.Rows.Add(row.Id, row.Bid, row.I, row.J, row.K, row.Xortho, row.Yortho, row.Zortho);
                    }
                    System.Data.SqlClient.SqlBulkCopy bcp =
                        new SqlBulkCopy(connectionString, SqlBulkCopyOptions.UseInternalTransaction)
                        {
                            BatchSize = 500,
                            DestinationTableName = "BOData_Computed_" + Context.ProjectId + "_" + Context.ModelId,
                            NotifyAfter = 500
                        };
                    bcp.SqlRowsCopied += (sender, e) =>
                    {
                        Console.WriteLine("Computed lines written: " + e.RowsCopied.ToString());
                    };
                    bcp.WriteToServer(dt);
                }
            }).Start();
        }
        private void DropTable()
        {
            using (IDbConnection connection = getConnection())
            {
                String ddl = "drop table BOData_" + Context.ProjectId + "_" + Context.ModelId;
                String computed_ddl = "drop table BOData_Computed_" + Context.ProjectId + "_" + Context.ModelId;
                try
                {
                    connection.Execute(ddl);
                } catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                try
                {
                    connection.Execute(computed_ddl);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void CreateTable()
        {
            using (IDbConnection connection = getConnection())
            {
                String ddl = "create table BOData_" + Context.ProjectId + "_" + Context.ModelId + " ( " +
                    " Id INT IDENTITY(1,1) PRIMARY KEY, ";
                for (int i = 1; i < reader.Header.Length; i++) // Skipping first entry as Id is supposed to be the first one
                {
                    if (i == 1)
                    {
                        ddl += $" { reader.Header[i]} VARCHAR(100) ";
                    }
                    else
                    {
                        ddl += $", { reader.Header[i]} VARCHAR(100) ";
                    }
                }
                ddl += " ) ";

                String compute_ddl = "create table BOData_Computed_" + Context.ProjectId + "_" + Context.ModelId + " ( " +
                    " Id INT PRIMARY KEY," +
                    " Bid BIGINT," +
                    " I INT, " +
                    " J INT," +
                    " K INT," +
                    " Xortho DECIMAL(18,10)," +
                    " Yortho DECIMAL(18,10)," +
                    " Zortho DECIMAL(18,10)" +
                    ")";

                connection.Execute(ddl);
                connection.Execute(compute_ddl);
            }
        }
    }

    class Row
    {
        public int Id { get; set; }
        public long Bid { get; set; }
        public String X { get; set; }
        public String Y { get; set; }
        public String Z { get; set; }
        public Decimal Tonnage { get; set; }
        public Decimal Xortho { get; set; }
        public Decimal Yortho { get; set; }
        public Decimal Zortho { get; set; }
        public int I { get; set; }
        public int J { get; set; }
        public int K { get; set; }
    }
}
