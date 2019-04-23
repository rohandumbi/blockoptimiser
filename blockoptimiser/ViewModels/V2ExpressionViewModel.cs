using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace blockoptimiser.ViewModels
{
    public class V2ExpressionViewModel : Conductor<Object>
    {
        private ModelDataAccess ModelDAO;
        private ExpressionDataAccess ExpressionDAO;
        private List<Model> Models;
        public BindableCollection<Expression> Expressions { get; set; }
        public String ExpressionName { get; set; }
        public ExpressionModelMappingViewModel UserControlViewModel { get; set; }
        public V2ExpressionViewModel()
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

        public void ClickExpression(object e, MouseButtonEventArgs mouseButtonEventArgs)
        {
            // MessageBox.Show(e.Source.ToString());
            Expression SelectedExpression = (Expression)e;
            ActivateItem(new ExpressionModelMappingViewModel(SelectedExpression));
            //UserControlViewModel = new ExpressionModelMappingViewModel(SelectedExpression);
            //NotifyOfPropertyChange("UserControlViewModel");
        }
    }
}
