﻿using System.Collections.Generic;
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
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using blockoptimiser.Models;
using blockoptimiser.ViewModels;
using Caliburn.Micro;
using System.Dynamic;
using Microsoft.Msagl.GraphViewerGdi;
using Point = Microsoft.Msagl.Core.Geometry.Point;
using System.Collections;
using blockoptimiser.DataAccessClasses;
using System;

namespace blockoptimiser.Views
{
    /// <summary>
    /// Interaction logic for V2ProcessView.xaml
    /// </summary>
    public partial class V2ProcessView : UserControl
    {
        DockPanel Panel = new DockPanel();
        public V2ProcessView()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ActualWidth > 0) //Ensuring the parent user control is in screen
            {
                //Setting widths of the graph containers
                ProcessGraphContainer.Width = ((this.ActualWidth / 2) - 5);
                ProductJoinGraphContainer.Width = ((this.ActualWidth / 2) - 5);
                //Inserting the graphs in the containers
                ProcessGraphContainer.Content = new V2ProcessGraph();
                ProductJoinGraphContainer.Content = new V2ProductJoinGraph();
            }
        }
    }
}
