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
    /// Interaction logic for V2ProductJoinGraph.xaml
    /// </summary>
    public partial class V2ProductJoinGraph : UserControl
    {
        DockPanel Panel = new DockPanel();
        public V2ProductJoinGraph()
        {
            InitializeComponent();
            this.Content = Panel;
            Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GraphViewer graphViewer = new GraphViewer();
            graphViewer.BindToPanel(Panel);
            Graph graph = new Graph();

            graph.AddEdge("c", "D");
            graph.Attr.LayerDirection = LayerDirection.LR;
            graphViewer.Graph = graph; // throws exception
        }
    }
}
