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
                    product.GradeNames = connection.Query<String>($"select GradeName from ProductGradeMapping where productid = { product.Id } ").ToList();
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
                String insertQuery = $"insert into Product (ProjectId, Name)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @Name)";

                String insertMappingQuery = $"insert into ProductProcessMapping (ProductId, ProcessId) " +
                    $" VALUES(@ProductId, @ProcessId)";

                String insertGradeMappingQuery = $"insert into ProductGradeMapping (ProductId, GradeName) " +
                    $" VALUES(@ProductId, @GradeName)";

                newProduct.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newProduct.ProjectId,
                    newProduct.Name
                });

                foreach (int processId in newProduct.ProcessIds)
                {
                    connection.Execute(insertMappingQuery, new
                    {
                        ProductId = newProduct.Id,
                        processId
                    });
                }

                foreach (String GradeName in newProduct.GradeNames)
                {
                    connection.Execute(insertGradeMappingQuery, new
                    {
                        ProductId = newProduct.Id,
                        GradeName
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

        public void InsertGradeMapping(int ProductId, String GradeName)
        {
            using (IDbConnection connection = getConnection())
            {
                connection.Execute("insert into ProductGradeMapping (ProductId, GradeName) VALUES (ProductId, GradeName)", new
                {
                    ProductId,
                    GradeName
                });
            }
        }

        public void DeleteGradeMapping(int ProductId, String GradeName)
        {
            using (IDbConnection connection = getConnection())
            {
                connection.Execute($"delete from ProductGradeMapping where ProductId = { ProductId } AND GradeName = {GradeName} ");
            }
        }

        public void Delete(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                connection.Execute($"delete from ProductProcessMapping where ProductId = { Id }");
                connection.Execute($"delete from ProductGradeMapping where ProductId = { Id }");
                connection.Execute($"delete from Product where Id = { Id }");
            }
        }
    }
}
