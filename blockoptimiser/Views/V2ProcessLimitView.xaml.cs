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
    /// Interaction logic for V2ProcessLimitView.xaml
    /// </summary>
    public partial class V2ProcessLimitView : UserControl
    {
        public V2ProcessLimitView()
        {
            InitializeComponent();
            //temp hack
            Context.ScenarioId = 1;
            if (!(Context.ScenarioId > 0))
            {
                MessageBox.Show("Please select a period.");
                return;
            }
            this.DataContext = new ProcessLimitViewModel();
        }

        private void AddProcessLimit(object sender, RoutedEventArgs e)
        {
            ((ProcessLimitViewModel)this.DataContext).CreateProcessLimit();
        }
    }
}
