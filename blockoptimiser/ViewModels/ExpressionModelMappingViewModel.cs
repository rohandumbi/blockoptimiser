using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.ViewModels
{
    public class ExpressionModelMappingViewModel: Screen
    {
        public String ExpressionName { get; set; }
        public BindableCollection<ExprModelMapping> ModelMapping { get; set; }
        private ModelDataAccess ModelDAO;
        private ExpressionDataAccess ExpressionDAO;
        private List<Model> Models;
        private List<Expression> Expressions;
        private int ExpressionId;
        private Expression UpdatedExpression;
        public ExpressionModelMappingViewModel(Expression Expression)
        {
            ExpressionDAO = new ExpressionDataAccess();
            ModelDAO = new ModelDataAccess();
            ExpressionId = Expression.Id;
            Models = ModelDAO.GetAll(Context.ProjectId);
            CreateMappingForMissingModels(Expression);
            Expressions = ExpressionDAO.GetAll(Context.ProjectId);
            UpdatedExpression = GetExpressionById(ExpressionId);
            ExpressionName = UpdatedExpression.Name;
            foreach (ExprModelMapping mapping in UpdatedExpression.modelMapping)
            {
                mapping.ModelName = GetModelById(mapping.ModelId).Name;
            }
            ModelMapping = new BindableCollection<ExprModelMapping>(UpdatedExpression.modelMapping);
        }

        private void CreateMappingForMissingModels(Expression expression)
        {
            List<ExprModelMapping> existingMapping = expression.modelMapping;
            foreach (Model model in Models)
            {
                //Geotech geotech = GeotechDAO.Get(model.Id);
                Boolean isEntryPresent = false;
                foreach (ExprModelMapping mapping in existingMapping)
                {
                    if (mapping.ModelId == model.Id)
                    {
                        isEntryPresent = true;
                        break;
                    }
                }
                if (!isEntryPresent)
                {
                    //creating default geotech against model, type field and first field selected by default
                    ExprModelMapping NewMapping = new ExprModelMapping();
                    NewMapping.ExprId = expression.Id;
                    NewMapping.ModelId = model.Id;
                    NewMapping.ModelName = model.Name;
                    NewMapping.ExprString = "";
                    expression.modelMapping.Add(NewMapping);
                    ExpressionDAO.InsertMapping(NewMapping);
                }
            }
        }

        private Expression GetExpressionById(int exprId)
        {
            Expression returnedExpression = null;
            foreach (Expression expression in Expressions)
            {
                if(expression.Id == exprId)
                {
                    returnedExpression = expression;
                    break;
                }
            }
            return returnedExpression;
        }

        private Model GetModelById(int modelId)
        {
            Model returnedModel = null;
            foreach (Model model in Models)
            {
                if (model.Id == modelId)
                {
                    returnedModel = model;
                    break;
                }
            }
            return returnedModel;
        }

        public void UpdateMapping(ExprModelMapping mapping)
        {
            ExpressionDAO.UpdateMapping(mapping);
        }
    }
}
