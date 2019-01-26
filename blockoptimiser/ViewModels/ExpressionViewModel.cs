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
    public class ExpressionViewModel: Screen
    {
        private ModelDataAccess ModelDAO;
        private ExpressionDataAccess ExpressionDAO;
        private List<Model> Models;
        private List<Expression> Expressions;
        public ExpressionViewModel()
        {
            Models = ModelDAO.GetAll(Context.ProjectId);
            Expressions = ExpressionDAO.GetAll(Context.ProjectId);
        }
    }
}
