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
    public class GradeLimitDataAccess : BaseDataAccess
    {
        public List<GradeLimit> GetGradeLimits()
        {
            using (IDbConnection connection = getConnection())
            {
                List<GradeLimit> GradeLimits = connection.Query<GradeLimit>($"select * from GradeLimit where ScenarioID = { Context.ScenarioId }").ToList();
                
                foreach (var GradeLimit in GradeLimits)
                {
                    GradeLimit.GradeLimitYearMapping
                        = connection.Query<GradeLimitYearMapping>($"select * from GradeLimitYearMapping where GradeLimitId = { GradeLimit.Id }").ToList();
                }
                return GradeLimits;
            }
        }

        public void Insert(GradeLimit newGradeLimit)
        {

            using (IDbConnection connection = getConnection())
            {
                String insertQuery = $"insert into GradeLimit (ScenarioId, IsMax, ItemName, ItemId, ItemType)" +
                    $" OUTPUT INSERTED.Id  " +
                    $" VALUES(@ScenarioId, @IsMax, @ItemName, @ItemId, @ItemType)";

                String insertMappingQuery = $"insert into GradeLimitYearMapping (GradeLimitId, Year, Value)" +
                    $" VALUES(@GradeLimitId, @Year, @Value)";

                newGradeLimit.Id = connection.QuerySingle<int>(insertQuery, new
                {
                    newGradeLimit.ScenarioId,
                    newGradeLimit.IsMax,
                    newGradeLimit.ItemName,
                    newGradeLimit.ItemId,
                    newGradeLimit.ItemType
                });

                foreach (GradeLimitYearMapping GradeLimitYearMapping in newGradeLimit.GradeLimitYearMapping)
                {
                    GradeLimitYearMapping.GradeLimitId = newGradeLimit.Id;
                    connection.Query(insertMappingQuery, new
                    {
                        GradeLimitYearMapping.GradeLimitId,
                        GradeLimitYearMapping.Year,
                        GradeLimitYearMapping.Value
                    });
                }
            }
        }

        public void InsertGradeLimitMapping(GradeLimit gradeLimit)
        {

            using (IDbConnection connection = getConnection())
            {

                String insertMappingQuery = $"insert into GradeLimitYearMapping (ProcessLimitId, Year, Value)" +
                    $" VALUES(@ProcessLimitId, @Year, @Value)";

                foreach (GradeLimitYearMapping GradeLimitYearMapping in gradeLimit.GradeLimitYearMapping)
                {
                    GradeLimitYearMapping.GradeLimitId = gradeLimit.Id;
                    connection.Query(insertMappingQuery, new
                    {
                        GradeLimitYearMapping.GradeLimitId,
                        GradeLimitYearMapping.Year,
                        GradeLimitYearMapping.Value
                    });
                }
            }
        }

        public void DeleteGradeLimitMapping(int GradeLimitId)
        {

            using (IDbConnection connection = getConnection())
            {

                String deleteMappingQuery = $"delete from GradeLimitYearMapping where GradeLimitId = {GradeLimitId} ";
                connection.Execute(deleteMappingQuery);
            }
        }
    }
}
