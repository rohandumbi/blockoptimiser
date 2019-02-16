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
    public class ProductDataAccess : BaseDataAccess
    {
        public List<Product> GetAll(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<Product>($"select * from Product where ProjectId = { ProjectId } ").ToList();
            }
        }

        public List<String> GetDistinctProductNames(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<String>($"select distinct Name from product where ProjectId = { ProjectId } ").ToList();
            }
        }

        public void Insert(Product newProduct)
        {
            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into Product (ProjectId, Name, AssociatedProcessId, UnitType, UnitId)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @Name, @AssociatedProcessId, @UnitType, @UnitId)";

                connection.Execute(insertQuery, new
                {
                    newProduct.ProjectId,
                    newProduct.Name,
                    newProduct.AssociatedProcessId,
                    newProduct.UnitType,
                    newProduct.UnitId
                });

            }
        }
        public void Delete(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                connection.Execute($"delete from Product where Id = { Id }");
            }
        }
    }
}
