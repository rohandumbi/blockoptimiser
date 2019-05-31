using blockoptimiser.ViewModels;
using Caliburn.Micro;
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
        private IEventAggregator _eventAggregator;
        public V2PeriodView(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            InitializeComponent();
            Loaded += Control_Loaded;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ActualWidth > 0) //ensuring control is in screen
            {
                this.DataContext = new V2PeriodViewModel(_eventAggregator);
            }
        }

        private void AddScenario(object sender, RoutedEventArgs e)
        {
            ((V2PeriodViewModel)this.DataContext).CreateScenario();
        }
    }
}
