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
    /// Interaction logic for V2LimitsView.xaml
    /// </summary>
    public partial class V2LimitsView : UserControl, IHandle<object>
    {
        private readonly IEventAggregator _eventAggregator;
        public V2LimitsView()
        {
            InitializeComponent();
            _eventAggregator = new EventAggregator();
            _eventAggregator.Subscribe(this);
            Loaded += Control_Loaded;
        }

        public void Handle(object message)
        {
            String EventName = message as String;
            if (EventName == "changed:scenario")
            {
                LoadLimits();
            }
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            PeriodViewContent.Content = new V2PeriodView(_eventAggregator);
        }

        private void LoadLimits()
        {
            FinanceLimitContent.Content = new V2FinanceLimitView();
            ProcessLimitContent.Content = new V2ProcessLimitView();
            GradeLimitContent.Content = new V2GradeLimitView();
            BenchLimitContent.Content = new V2BenchLimitView();
        }
    }
}
