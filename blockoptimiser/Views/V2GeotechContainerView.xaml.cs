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
    /// Interaction logic for V2GeotechContainerView.xaml
    /// </summary>
    public partial class V2GeotechContainerView : UserControl
    {
        public V2GeotechContainerView()
        {
            InitializeComponent();
            Loaded += Control_Loaded;
            IsVisibleChanged += Visibility_Handler;
        }

        private void Visibility_Handler(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((bool)e.NewValue)) //ensuring control is in screen
            {
                this.DataContext = new V2GeotechContainerViewModel();
                InitializeComponent();
            }
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ActualWidth > 0) //ensuring control is in screen
            {
                this.DataContext = new V2GeotechContainerViewModel();
            }
        }

        private void TabClick(object sender, RoutedEventArgs e)
        {
            var ctx = (V2GeotechContainerViewModel)this.DataContext;
            ctx.ClickTab(sender);
        }
    }
}
