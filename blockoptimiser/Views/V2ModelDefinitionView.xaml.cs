﻿using blockoptimiser.ViewModels;
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
            this.DataContext = new ModelDefinitionViewModel();
        }

        private void InputFile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                InputFile.Text = openFileDialog.FileName;
        }
    }
}
