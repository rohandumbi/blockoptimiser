using blockoptimiser.ViewModels;
using Microsoft.Win32;
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
    /// Interaction logic for V2ModelDefinitionView.xaml
    /// </summary>
    public partial class V2ModelDefinitionView : UserControl
    {
        public V2ModelDefinitionView()
        {
            InitializeComponent();
            Loaded += Control_Loaded;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ActualWidth > 0) //ensuring control is in screen
            {
                this.DataContext = new ModelDefinitionViewModel();
                CSVDataGrid.MaxHeight = (this.ActualHeight - groupBox.ActualHeight) - 50;
                DimensionGrid.MaxHeight = CSVDataGrid.MaxHeight;
            }
        }

        private void Import_Clicked(object sender, RoutedEventArgs e)
        {
            ((ModelDefinitionViewModel)this.DataContext).ImportData();
        }

        private void InputFile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                InputFileTextBox.Text = openFileDialog.FileName;
                ((ModelDefinitionViewModel)this.DataContext).FileChosen(openFileDialog.FileName);
            }
        }
    }
}
