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
    public class CsvColumnMappingDataAccess : BaseDataAccess
    {
        public List<CsvColumnMapping> GetAll(int ModelId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<CsvColumnMapping>($"select a.*, b.DataType, b.AssociatedField " +
                    $"from CsvColumnMapping a, field b where b.Id = a.FieldId and a.ModelId = { ModelId }").ToList();
            }
        }

        public void Insert(CsvColumnMapping newCsvColumnMapping)
        {

            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into CsvColumnMapping (ModelId, ColumnName, FieldId, DefaultValue)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ModelId, @ColumnName, @FieldId, @DefaultValue)";
                newCsvColumnMapping.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newCsvColumnMapping.ModelId,
                    newCsvColumnMapping.ColumnName,
                    newCsvColumnMapping.FieldId,
                    newCsvColumnMapping.DefaultValue
                });
            }
        }

        public void DeleteAll(int ModelId)
        {
            using (IDbConnection connection = getConnection())
            {
                String deleteQuery = $"delete from CsvColumnMapping where ModelId = { ModelId }";
                connection.Execute(deleteQuery);
            }
        }
        public void Delete(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                String deleteQuery = $"delete from CsvColumnMapping where Id = { Id }";
                connection.Execute(deleteQuery);
            }
        }
    }
}
