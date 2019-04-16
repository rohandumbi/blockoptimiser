using blockoptimiser.ViewModels;
using MahApps.Metro.Controls;
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
    /// Interaction logic for V2AppView.xaml
    /// </summary>
    public partial class V2AppView : UserControl
    {
        public V2AppView()
        {
            InitializeComponent();
            //this.DataContext = new GeotechContainerViewModel();
        }
        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            //this.DataContext = new GeotechContainerViewModel();
            //HamburgerMenuControl.Content = e.InvokedItem;
            HamburgerMenuControl.Content = e.InvokedItem;

            //if ((e.InvokedItem as HamburgerMenuIconItem).Tag != null)
            //{
            //    ((e.InvokedItem as HamburgerMenuIconItem).Tag as UserControl).DataContext = new GeotechContainerView();
            //}
            //HamburgerMenuControl.Content = e.InvokedItem;
        }
        //HamburgerMenuControl.Content = new GeotechContainerView();
    }
}
