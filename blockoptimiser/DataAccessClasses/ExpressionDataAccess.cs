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

        public Expression Get(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                var expression = connection.QuerySingle<Expression>($"select * from expression where Id = { Id } ");
                expression.modelMapping
                        = connection.Query<ExprModelMapping>($"select *, b.Name from ExprModelMapping a, Model b " +
                        $"where b.Id = a.ModelId and ExprId = { Id }").ToList();
                
                return expression;
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

                    // Update computed data as well. this is not a good place but I need to find a better place
                    try
                    {
                        if (String.IsNullOrEmpty(ModelMapping.ExprString))
                        {
                            ModelMapping.ExprString = "0";
                        }
                        connection.Execute($"ALTER TABLE BOData_Computed_{ newExpression.ProjectId }_{ ModelMapping.ModelId }" +
                       $" ADD { newExpression.Name } DECIMAL(18,10) ");
                        String updateQuery = $" update BOData_Computed_{ newExpression.ProjectId }_{ ModelMapping.ModelId }" +
                             $" set { newExpression.Name } = ( select ISNULL( {ModelMapping.ExprString} , 0) from BOData_{ newExpression.ProjectId }_{ ModelMapping.ModelId } a " +
                             $"where a.Id = BOData_Computed_{ newExpression.ProjectId }_{ ModelMapping.ModelId }.Id )";
                        connection.Execute(updateQuery);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                   
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

                Expression expression = this.Get(newMapping.ExprId);
                try
                {
                    if(String.IsNullOrEmpty(newMapping.ExprString))
                    {
                        newMapping.ExprString = "0";
                    }
                    connection.Execute($"ALTER TABLE BOData_Computed_{ expression.ProjectId }_{ newMapping.ModelId }" +
                   $" ADD { expression.Name } DECIMAL(18,10) ");
                    String updateQuery = $" update BOData_Computed_{ expression.ProjectId }_{ newMapping.ModelId }" +
                             $" set { expression.Name } = ( select ISNULL( {newMapping.ExprString} , 0) from BOData_{ expression.ProjectId }_{ newMapping.ModelId } a " +
                             $"where a.Id = BOData_Computed_{ expression.ProjectId }_{ newMapping.ModelId }.Id )";
                    connection.Execute(updateQuery);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                
            }
        }

        public void UpdateMapping(ExprModelMapping updatedMapping)
        {
            using (IDbConnection connection = getConnection())
            {
                String updateQuery = $"update ExprModelMapping set ExprString = @ExprString where ExprId = @ExprId AND ModelId = @ModelId";
                connection.Execute(updateQuery, new
                {
                    updatedMapping.ExprString,
                    updatedMapping.ExprId,
                    updatedMapping.ModelId
                });
                try
                {
                    if (String.IsNullOrEmpty(updatedMapping.ExprString))
                    {
                        updatedMapping.ExprString = "0";
                    }
                    Expression expression = this.Get(updatedMapping.ExprId);
                    String sql = $" update BOData_Computed_{ expression.ProjectId }_{ updatedMapping.ModelId }" +
                        $" set { expression.Name } = ( select ISNULL( {updatedMapping.ExprString} , 0) from BOData_{ expression.ProjectId }_{ updatedMapping.ModelId } a " +
                        $"where a.Id = BOData_Computed_{ expression.ProjectId }_{ updatedMapping.ModelId }.Id )";
                    connection.Execute(sql);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
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
                    try
                    {
                        if (String.IsNullOrEmpty(ModelMapping.ExprString))
                        {
                            ModelMapping.ExprString = "0";
                        }
                        connection.Execute($"ALTER TABLE BOData_Computed_{ expression.ProjectId }_{ ModelMapping.ModelId }" +
                       $" ADD { expression.Name } DECIMAL(18,10) ");
                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    String updateQuery = $" update BOData_Computed_{ expression.ProjectId }_{ ModelMapping.ModelId }" +
                        $" set { expression.Name } = ( select ISNULL( {ModelMapping.ExprString} , 0) from BOData_{ expression.ProjectId }_{ ModelMapping.ModelId } a " +
                        $"where a.Id = BOData_Computed_{ expression.ProjectId }_{ ModelMapping.ModelId }.Id )";
                    connection.Execute(updateQuery);
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
