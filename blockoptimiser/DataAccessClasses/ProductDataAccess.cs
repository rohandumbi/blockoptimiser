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
                List<Product> Products = connection.Query<Product>($"select * from Product where ProjectId = { ProjectId } ").ToList();
                foreach (Product product in Products)
                {
                    product.ProcessIds = connection.Query<int>($"select processId from ProductProcessMapping where productid = { product.Id } ").ToList();
                }
                return Products;
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
                String insertQuery = $"insert into Product (ProjectId, Name, UnitType, UnitId)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @Name, @UnitType, @UnitId)";

                connection.Execute(insertQuery, new
                {
                    newProduct.ProjectId,
                    newProduct.Name,
                    newProduct.UnitType,
                    newProduct.UnitId
                });

                foreach (int processId in newProduct.ProcessIds)
                {
                    connection.Execute("insert into ProductProcessMapping (ProductId, ProcessId) VALUES (ProductId, ProcessId)", new
                    {
                        newProduct.Id,
                        processId
                    });
                }

            }
        }

        public void InsertProcessMapping(int ProductId, int ProcessId)
        {
            using (IDbConnection connection = getConnection())
            {
                connection.Execute("insert into ProductProcessMapping (ProductId, ProcessId) VALUES (ProductId, ProcessId)", new
                {
                    ProductId,
                    ProcessId
                });
            }
        }

        public void DeleteProcessMapping(int ProductId, int ProcessId)
        {
            using (IDbConnection connection = getConnection())
            {
                connection.Execute($"delete from ProductProcessMapping where ProductId = { ProductId } AND ProcessId = {ProcessId} ");
            }
        }

        public void Delete(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                connection.Execute($"delete from ProductProcessMapping where ProductId = { Id }");
                connection.Execute($"delete from Product where Id = { Id }");
            }
        }
    }
}
