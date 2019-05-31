using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using blockoptimiser.Services.LP;
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
using System.Windows.Shapes;

namespace blockoptimiser.Views
{
    /// <summary>
    /// Interaction logic for SchedulerWindow.xaml
    /// </summary>
    public partial class V2SettingsView : UserControl
    {
        public List<String> AvailableYears { get; set; }
        public String StartYear { get; set; }
        public String EndYear { get; set; }
        public List<Scenario> AvailableScenarios { get; set; }

        public V2SettingsView()
        {
            InitializeComponent();
            Loaded += Control_Loaded;
            IsVisibleChanged += Visibility_Handler;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ActualWidth > 0) //ensuring control is in screen
            {
                this.DataContext = new V2SettingsViewModel();
            }
        }

        private void Visibility_Handler(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((bool)e.NewValue)) //ensuring control is in screen
            {
                this.DataContext = new V2SettingsViewModel();
                InitializeComponent();
            }
        }

        public void button_Click(object sender, RoutedEventArgs e)
        {
            ((V2SettingsViewModel)this.DataContext).RunScheduler();
        }
    }
}
