using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.ViewModels
{
    public class ZoneViewModel: Screen
    {
        public String ScreenDescription { get; set; }
        public ZoneViewModel()
        {
            ScreenDescription = "This is Zone View.";
        }
    }
}
