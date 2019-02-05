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
    public class ProductJoinDataAccess : BaseDataAccess
    {
        public List<ProductJoin> GetAll(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<ProductJoin>($"select * from productjoin where ProjectId = { ProjectId } ").ToList();
            }
        }
        public void Insert(ProductJoin newProductJoin)
        {
            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into ProductJoin (ProjectId, Name)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @Name)";

                newProductJoin.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newProductJoin.ProjectId,
                    newProductJoin.Name
                });

            }
        }

        public void Delete(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                connection.Execute($"delete from ProductJoin where Id = { Id }");
            }
        }
    }
}
