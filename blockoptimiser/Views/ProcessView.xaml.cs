using blockoptimiser.DataAccessClasses;
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
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;

namespace blockoptimiser.Views
{
    /// <summary>
    /// Interaction logic for ProcessView.xaml
    /// </summary>
    public partial class ProcessView : UserControl
    {
        Grid mainGrid = new Grid();
        DockPanel ProcessGraphViewerPanel = new DockPanel();
        DockPanel ProductGraphViewerPanel = new DockPanel();
        GraphViewer ProcessGraphViewer = new GraphViewer();
        GraphViewer ProductGraphViewer = new GraphViewer();

        ToolBar toolBar = new ToolBar();

        StackPanel sp1 = new StackPanel();
        public ProcessView()
        {
            InitializeComponent();
            this.Content = mainGrid;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            ProcessGraphViewerPanel.ClipToBounds = true;
            ProductGraphViewerPanel.ClipToBounds = true;

            mainGrid.Children.Add(toolBar);
            toolBar.VerticalAlignment = VerticalAlignment.Top;

            mainGrid.Children.Add(ProcessGraphViewerPanel);
            mainGrid.Children.Add(ProductGraphViewerPanel);

            //mainGrid.Children.Add(sp1);
            double height = this.Width;

            ProcessGraphViewer.BindToPanel(ProcessGraphViewerPanel);
            ProductGraphViewer.BindToPanel(ProductGraphViewerPanel);

            ProcessGraphViewerPanel.HorizontalAlignment = HorizontalAlignment.Left;
            ProductGraphViewerPanel.HorizontalAlignment = HorizontalAlignment.Right;

            ProcessGraphViewerPanel.Width = (this.ActualWidth/2 - 10);
            ProductGraphViewerPanel.Width = (this.ActualWidth/2 - 10);

            ProcessGraphViewerPanel.Height = this.ActualHeight - 20;
            ProductGraphViewerPanel.Height = this.ActualHeight - 20;

            //ProcessGraphViewer.BindToPanel(ProcessGraphViewerPanel);
            //ProductGraphViewer.BindToPanel(ProcessGraphViewerPanel);

            //ProcessGraphViewerPanel.ClipToBounds = true;
            //ProductGraphViewerPanel.ClipToBounds = true;

            //ProcessGraphViewerPanel.VerticalAlignment = VerticalAlignment.Top;
            //ProductGraphViewerPanel.VerticalAlignment = VerticalAlignment.Bottom;


            Graph ProcessGraph = new Graph();
            Graph ProductGraph = new Graph();

            //Process Graph
            ProcessGraph.AddEdge("A", "B");
            ProcessGraph.Attr.LayerDirection = LayerDirection.LR;
            ProcessGraphViewer.Graph = ProcessGraph; // throws exception

            //Product Graph
            ProductGraph.AddEdge("C", "D");
            ProductGraph.Attr.LayerDirection = LayerDirection.LR;
            ProductGraphViewer.Graph = ProductGraph; // throws exception
        }

        void SetMainMenu()
        {
            var mainMenu = new Menu { IsMainMenu = true };
            toolBar.Items.Add(mainMenu);
            //SetFileMenu(mainMenu);
            //SetViewMenu(mainMenu);
        }
    }
}
