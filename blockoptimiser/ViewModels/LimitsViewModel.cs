using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.ViewModels
{
    public class LimitsViewModel: Screen
    {
        public String ScreenDescription { get; set; }
        public LimitsViewModel()
        {
            ScreenDescription = "This is Limits View.";
        }
    }
}
