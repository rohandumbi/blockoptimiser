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
    public class FieldDataAccess : BaseDataAccess
    {

        public List<Field> GetAll(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<Field>($"select * from field where ProjectId = { ProjectId } ").ToList();
            }
        }

        public void Insert(Field newField)
        {

            using (IDbConnection connection = getConnection())
            {
                if(newField.DataType == Field.DATA_TYPE_GRADE)
                {
                    Console.WriteLine("Associated field "+newField.AssociatedField);
                }
                
                String insertQuery = $"insert into field (ProjectId, Name, DataType, AssociatedField)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @Name, @DataType, @AssociatedField)";
                newField.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newField.ProjectId,
                    newField.Name,
                    newField.DataType,
                    newField.AssociatedField
                });
            }
        }

        public void Update(Field _field)
        {

            using (IDbConnection connection = getConnection())
            {
                String updateQuery = $"update field set DataType = @DataType, AssociatedField = @AssociatedField where Id = @Id ";
                connection.Execute(updateQuery, new
                {
                    _field.DataType,
                    _field.AssociatedField,
                    _field.Id
                });
            }
        }
        public void DeleteAll(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                String deleteQuery = $"delete from field where ProjectId = { ProjectId }";
                connection.Execute(deleteQuery);
            }
        }
        public void Delete(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                String deleteQuery = $"delete from field where Id = { Id }";
                connection.Execute(deleteQuery);
            }
        }
    }
}
