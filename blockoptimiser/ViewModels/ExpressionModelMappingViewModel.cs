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
        public ExpressionModelMappingViewModel(Expression Expression)
        {
            ExpressionName = Expression.Name;
        }
    }
}
