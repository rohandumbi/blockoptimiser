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
    /// Interaction logic for V2ProcessGraph.xaml
    /// </summary>
    public partial class V2ProcessGraph : UserControl
    {
        DockPanel Panel = new DockPanel();
        GraphViewer graphViewer;
        Graph graph;
        List<Product> Products;
        List<Process> Processes;
        ProcessDataAccess ProcessDAO;
        ProductDataAccess ProductDAO;
        public V2ProcessGraph()
        {
            InitializeComponent();
            this.Content = Panel;
            ProcessDAO = new ProcessDataAccess();
            ProductDAO = new ProductDataAccess();
            Processes = ProcessDAO.GetAll(Context.ProjectId);
            Products = ProductDAO.GetAll(Context.ProjectId);
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Panel.ClipToBounds = true;
            graphViewer = new GraphViewer();
            graphViewer.BindToPanel(Panel);
            graph = new Graph();
            graph.Attr.LayerDirection = LayerDirection.TB;
            AddProcessNodes();
            AddProductNodes();
            graphViewer.Graph = graph; // throws exception
        }
        private void AddProcessNodes()
        {
            foreach (Process process in Processes)
            {
                graph.AddNode(process.Name);
            }
        }

        private void AddProductNodes()
        {
            foreach (Product product in Products)
            {
                graph.AddNode(product.Name);
                foreach (int ProcessId in product.ProcessIds)
                {
                    graph.AddEdge(product.Name, GetProcessById(ProcessId).Name);
                }

            }
        }

        private Process GetProcessById(int Id)
        {
            Process selectedProcess = new Process();
            foreach (Process process in Processes)
            {
                if (process.Id == Id)
                {
                    selectedProcess = process;
                    break;
                }
            }
            return selectedProcess;
        }
    }
}
