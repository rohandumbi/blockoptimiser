using blockoptimiser.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace blockoptimiser.DataAccessClasses
{
    public class ProjectDataAccess : BaseDataAccess
    {
        public List<Project> GetAll()
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<Project>("select * from project").ToList();
            }
        }

        public Project Get(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                String selectQuery = $"select * from Project where Id = { ProjectId }";
                return connection.Query<Project>(selectQuery).ToList().First();
            }
        }

        public void Insert(Project newProject)
        {

            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into Project (Name, Description, CreatedDate, ModifiedDate)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@Name, @Description, GETDATE(), GETDATE())";
                newProject.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newProject.Name,
                    newProject.Description
                });
            }
        }

        public void Delete(int ProjectId)
        {
            using (IDbConnection connection = getConnection())
            {
                String deleteQuery = $"delete from Project where Id = { ProjectId }";
                connection.Execute(deleteQuery);
            }
        }

        public void Clone(int ProjectId)
        {
            MessageBox.Show("TODO: DB implementation of cloning project id: " + ProjectId);
        }
    }
}
