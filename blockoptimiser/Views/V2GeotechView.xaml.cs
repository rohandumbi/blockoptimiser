using blockoptimiser.Models;
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
    /// Interaction logic for V2GeotechView.xaml
    /// </summary>
    public partial class V2GeotechView : UserControl
    {
        public V2GeotechView()
        {
            InitializeComponent();
            this.DataContext = new GeotechViewModel();
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            var ctx = (GeotechViewModel)this.DataContext;
            ctx.UpdateGeotech(sender as Geotech);
        }
    }
}
