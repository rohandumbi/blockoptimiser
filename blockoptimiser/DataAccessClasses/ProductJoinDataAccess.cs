﻿using blockoptimiser.Models;
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

        public List<String> GetProductJoins(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<String>($"select distinct Name from productjoin where ProjectId = { ProjectId } ").ToList();
            }
        }

        public List<String> GetProductsInJoin(String ProductJoinName)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<String>($"select ProductName from productjoin where Name = '{ ProductJoinName }' ").ToList();
            }
        }

        public void Insert(ProductJoin newProductJoin)
        {
            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into ProductJoin (ProjectId, Name, ProductName)" +
                    $" VALUES(@ProjectId, @Name, @ProductName)";

                foreach(String ProductName in newProductJoin.ProductNames)
                {
                    connection.Execute(insertQuery, new
                    {
                        newProductJoin.ProjectId,
                        newProductJoin.Name,
                        ProductName
                    });
                }
                

            }
        }

        public void Delete(String ProductJoinName)
        {
            using (IDbConnection connection = getConnection())
            {
                connection.Execute($"delete from ProductJoin where Name = '{ ProductJoinName }'");
            }
        }

        public void DeleteProduct(String ProductJoinName, String ProductName)
        {
            using (IDbConnection connection = getConnection())
            {
                connection.Execute($"delete from ProductJoin where Name = '{ ProductJoinName }' AND ProductName = '{ProductName}' ");
            }
        }
    }
}
