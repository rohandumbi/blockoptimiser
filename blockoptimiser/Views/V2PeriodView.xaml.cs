using blockoptimiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace blockoptimiser.Views
{
    /// <summary>
    /// Interaction logic for V2PeriodView.xaml
    /// </summary>
    public partial class V2PeriodView : UserControl
    {
        public V2PeriodView()
        {
            InitializeComponent();
            //temp hack
            Context.ScenarioId = 1;
            this.DataContext = new V2PeriodViewModel();
        }
        private void AddScenario(object sender, RoutedEventArgs e)
        {
            ((V2PeriodViewModel)this.DataContext).CreateScenario();
        }
    }
}
