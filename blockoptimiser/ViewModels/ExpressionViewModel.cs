using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.ViewModels
{
    public class ExpressionViewModel: Screen
    {
        private ModelDataAccess ModelDAO;
        private ExpressionDataAccess ExpressionDAO;
        private List<Model> Models;
        public BindableCollection<Expression> Expressions { get; set; }
        public String ExpressionName { get; set; }
        public ExpressionViewModel()
        {
            ExpressionDAO = new ExpressionDataAccess();
            ModelDAO = new ModelDataAccess();
            Expressions = new BindableCollection<Expression>(ExpressionDAO.GetAll(Context.ProjectId));
            Models = ModelDAO.GetAll(Context.ProjectId);
        }

        public void AddExpression()
        {
            Expression NewExpression = new Expression
            {
                ProjectId = Context.ProjectId,
                Name = ExpressionName,
                modelMapping = GetDefaultModelMapping()
            };
            ExpressionDAO.Insert(NewExpression);
            Expressions.Add(NewExpression);
            NotifyOfPropertyChange("Expressions");
        }

        private List<ExprModelMapping> GetDefaultModelMapping()
        {
            List<ExprModelMapping> DefaultMapping = new List<ExprModelMapping>();
            foreach (Model Model in Models)
            {
                ExprModelMapping NewMapping = new ExprModelMapping();
                NewMapping.ModelId = Model.Id;
                NewMapping.ModelName = Model.Name;
                NewMapping.ExprString = "";
                DefaultMapping.Add(NewMapping);
            }

            return DefaultMapping;
        }
    }
}
