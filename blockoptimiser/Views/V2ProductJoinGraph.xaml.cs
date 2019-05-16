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
        GraphViewer graphViewer;
        Graph graph;
        List<Product> Products;
        List<String> ProductJoinNames;
        ProductDataAccess ProductDAO;
        ProductJoinDataAccess ProductJoinDAO;
        public V2ProductJoinGraph()
        {
            InitializeComponent();
            this.Content = Panel;
            ProductDAO = new ProductDataAccess();
            ProductJoinDAO = new ProductJoinDataAccess();
            Products = ProductDAO.GetAll(Context.ProjectId);
            ProductJoinNames = ProductJoinDAO.GetProductJoins(Context.ProjectId);
            Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Panel.ClipToBounds = true;
            graphViewer = new GraphViewer();
            graphViewer.BindToPanel(Panel);
            graph = new Graph();
            graph.Attr.LayerDirection = LayerDirection.TB;
            AddProductNodes();
            AddProductJoinNodes();
            graphViewer.Graph = graph; // throws exception
        }

        private void AddProductNodes()
        {
            foreach (Product product in Products)
            {
                graph.AddNode(product.Name);
            }
        }

        private void AddProductJoinNodes()
        {
            foreach (String productJoinName in ProductJoinNames)
            {
                graph.AddNode(productJoinName);
                List<String> ProductNames = ProductJoinDAO.GetProductsInJoin(productJoinName);
                foreach (String productName in ProductNames)
                {
                    graph.AddEdge(productName, productJoinName);
                }
            }
        }
    }
}
