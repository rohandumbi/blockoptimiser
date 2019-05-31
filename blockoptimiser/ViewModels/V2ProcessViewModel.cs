using blockoptimiser.Views;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace blockoptimiser.ViewModels
{
    public class V2ProcessViewModel: Screen
    {
        public UserControl GraphControl { get; set; }
        public V2ProcessViewModel()
        {
            GraphControl = new ProcessView();
        }
    }
}
