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
    public class ModelDimensionDataAccess : BaseDataAccess
    {
        public List<ModelDimension> GetAll()
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<ModelDimension>("select * from modeldimension").ToList();
            }
        }

        public List<ModelDimension> GetAll(int ModelId)
        {
            using (IDbConnection connection = getConnection())
            {
                return connection.Query<ModelDimension>("select * from modeldimension where ModelId=" + ModelId).ToList();
            }
        }

        public void Insert(ModelDimension newModelDimension)
        {

            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into modeldimension (ModelId, Type, XDim, YDim, ZDim)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ProjectId, @Name)";
                newModelDimension.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newModelDimension.ModelId,
                    newModelDimension.Type,
                    newModelDimension.XDim,
                    newModelDimension.YDim,
                    newModelDimension.ZDim
                });
            }
        }

        public void Delete(int Id)
        {
            using (IDbConnection connection = getConnection())
            {
                String deleteQuery = $"delete from modeldimension where Id = { Id }";
                connection.Execute(deleteQuery);
            }
        }
    }
}
