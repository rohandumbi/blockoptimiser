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
    /// Interaction logic for V2ExpressionModelMappingView.xaml
    /// </summary>
    public partial class V2ExpressionModelMappingView : UserControl
    {
        private blockoptimiser.Models.Expression _expresion;
        public V2ExpressionModelMappingView(blockoptimiser.Models.Expression expression)
        {
            _expresion = expression;
            InitializeComponent();
            Loaded += Control_Loaded;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ActualWidth > 0) //ensuring control is in screen
            {
                this.DataContext = new V2ExpressionModelMappingViewModel(_expresion);
                Wrapper.MaxHeight = this.ActualHeight - 20;
            }
        }

        private void ExpressionChanged(object sender, RoutedEventArgs e)
        {
            var ctx = (V2ExpressionModelMappingViewModel)this.DataContext;
            ctx.UpdateMapping(((TextBox)sender).DataContext as ExprModelMapping);
        }
    }
}
