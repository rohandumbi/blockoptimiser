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
    /// Interaction logic for V2ModelImportView.xaml
    /// </summary>
    public partial class V2ModelImportView : UserControl
    {
        public V2ModelImportView()
        {
            InitializeComponent();
            this.DataContext = new V2ModelImportViewModel();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ActualWidth > 0) //ensuring control is in screen
            {
                this.DataContext = new V2ModelImportViewModel();
            }
        }

        private void HandleMenu(object sender, RoutedEventArgs e)
        {
            ((V2ModelImportViewModel)this.DataContext).ClickMenu(sender, (MouseButtonEventArgs)e);
        }

        private void AddModel(object sender, RoutedEventArgs e)
        {
            ((V2ModelImportViewModel)this.DataContext).AddModel();
        }
    }
}
