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
using blockoptimiser.Models;

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
        Graph ProcessGraph = new Graph();
        Graph ProductGraph = new Graph();

        ToolBar toolBar = new ToolBar();
        ProductDataAccess ProductDAO;
        ProductJoinDataAccess ProductJoinDAO;

        List<Product> Products;
        List<ProductJoin> ProductJoins;

        StackPanel sp1 = new StackPanel();
        public ProcessView()
        {
            InitializeComponent();
            ProductDAO = new ProductDataAccess();
            ProductJoinDAO = new ProductJoinDataAccess();
            Products = ProductDAO.GetAll(Context.ProjectId);
            ProductJoins = ProductJoinDAO.GetAll(Context.ProjectId);
            mainGrid.Background = Brushes.White;
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



            // Creating the Product Graph
            AddProductNodes();
            AddProductJoinNodes();
            ProductGraph.Attr.LayerDirection = LayerDirection.LR;
            ProductGraphViewer.Graph = ProductGraph;

        }

        private void AddProductNodes()
        {
            foreach (Product product in Products)
            {
                ProductGraph.AddNode(product.Name);
            }
        }

        private void AddProductJoinNodes()
        {
            foreach (ProductJoin productjoin in ProductJoins)
            {
                ProductGraph.AddNode(productjoin.Name);
                ProductGraph.AddEdge(GetProductById(productjoin.ChildProductId).Name, productjoin.Name);
            }
        }

        private Product GetProductById(int Id)
        {
            Product selectedProduct = new Product();
            foreach (Product product in Products)
            {
                if (product.Id == Id)
                {
                    selectedProduct = product;
                    break;
                }
            }
            return selectedProduct;
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
