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
    /// Interaction logic for V2ExpressionView.xaml
    /// </summary>
    public partial class V2ExpressionView : UserControl
    {
        public V2ExpressionView()
        {
            InitializeComponent();
            this.DataContext = new ExpressionViewModel();
        }

        private void MouseLeftUp(object sender, RoutedEventArgs e)
        {
            var ctx = (ExpressionViewModel)this.DataContext;
            ctx.ClickExpression(((TextBlock)sender).DataContext, e as MouseButtonEventArgs);
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            var ctx = (ExpressionViewModel)this.DataContext;
            ctx.AddExpression();
        }
    }
}
