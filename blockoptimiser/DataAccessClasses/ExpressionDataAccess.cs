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
    public class ExpressionDataAccess : BaseDataAccess
    {
        public List<Expression> GetAll(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                var Expressions = connection.Query<Expression>($"select * from expression where ProjectId = { ProjectId } ").ToList();
                foreach (var Expression in Expressions)
                {
                    Expression.modelMapping 
                        = connection.Query<ExprModelMapping>($"select *, b.Name from ExprModelMapping a, Model b " +
                        $"where b.Id = a.ModelId and ExprId = { Expression.Id }").ToList();
                }
                return Expressions;
            }
        }

        public void Insert(Expression newExpression)
        {
            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into Expression (ProjectId, Name)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @Name)";
                String insertMappingQuery = $"insert into ExprModelMapping (ExprId, ModelId, ExprString)" +
                    $" VALUES(@ExprId, @ModelId, @ExprString)";
                newExpression.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newExpression.ProjectId,
                    newExpression.Name
                });

                foreach (ExprModelMapping ModelMapping in newExpression.modelMapping)
                {
                    ModelMapping.ExprId = newExpression.Id;
                    connection.Query(insertMappingQuery, new
                    {
                        ModelMapping.ExprId,
                        ModelMapping.ModelId,
                        ModelMapping.ExprString
                    });
                }
            }
        }
        public void InsertMapping(ExprModelMapping newMapping)
        {
            using (IDbConnection connection = getConnection())
            {
                
                String insertMappingQuery = $"insert into ExprModelMapping (ExprId, ModelId, ExprString)" +
                    $" VALUES(@ExprId, @ModelId, @ExprString)";

                connection.Query(insertMappingQuery, new
                {
                    newMapping.ExprId,
                    newMapping.ModelId,
                    newMapping.ExprString
                });
            }
        }

        public void InsertModelMapping(Expression expression)
        {
            using (IDbConnection connection = getConnection())
            {
                String insertMappingQuery = $"insert into ExprModelMapping (ExprId, ModelId, ExprString)" +
                    $" VALUES(@ExprId, @ModelId, @ExprString)";

                foreach (ExprModelMapping ModelMapping in expression.modelMapping)
                {
                    ModelMapping.ExprId = expression.Id;
                    connection.Query(insertMappingQuery, new
                    {
                        ModelMapping.ExprId,
                        ModelMapping.ModelId,
                        ModelMapping.ExprString
                    });
                }
            }
        }

        public void Update(Expression expression)
        {

            using (IDbConnection connection = getConnection())
            {
                String updateQuery = $"update Expression set Name = { expression.Name } where Id = { expression.Id } ";
                connection.Execute(updateQuery);
            }
        }

        public void Delete(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                String deleteQuery = $"delete from Expression where Id = { Id }";
                String deleteMappingQuery = $"delete from ExprModelMapping where ExprId = { Id }";
                connection.Execute(deleteMappingQuery);
                connection.Execute(deleteQuery);
            }
        }
    }
}
