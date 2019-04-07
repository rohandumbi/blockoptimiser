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
    /// Interaction logic for V2ProjectsView.xaml
    /// </summary>
    public partial class V2ProjectsView : UserControl
    {
        public V2ProjectsView()
        {
            InitializeComponent();
        }

        private void Add_Project(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add new project");
        }
    }
}
