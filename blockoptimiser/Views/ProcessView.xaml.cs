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
using blockoptimiser.ViewModels;
using Caliburn.Micro;
using System.Dynamic;

namespace blockoptimiser.Views
{
    /// <summary>
    /// Interaction logic for ProcessView.xaml
    /// </summary>
    public partial class ProcessView : UserControl
    {
        Grid mainGrid = new Grid();
        Border ProcessGraphBorder = new Border();
        Border ProductGraphBorder = new Border();
        DockPanel ProcessGraphViewerPanel = new DockPanel();
        DockPanel ProductGraphViewerPanel = new DockPanel();
        GraphViewer ProcessGraphViewer = new GraphViewer();
        GraphViewer ProductGraphViewer = new GraphViewer();
        Graph ProcessGraph = new Graph();
        Graph ProductGraph = new Graph();

        ToolBar toolBar = new ToolBar();
        ProductDataAccess ProductDAO;
        ProductJoinDataAccess ProductJoinDAO;
        ProcessDataAccess ProcessDAO;

        List<Product> Products;
        List<ProductJoin> ProductJoins;
        List<Process> Processes;
        List<String> ProductJoinNames;

        StackPanel sp1 = new StackPanel();

        IWindowManager WindowManager;
        public ProcessView()
        {
            InitializeComponent();
            ProductDAO = new ProductDataAccess();
            ProductJoinDAO = new ProductJoinDataAccess();
            ProcessDAO = new ProcessDataAccess();
            Products = ProductDAO.GetAll(Context.ProjectId);
            ProductJoinNames = ProductJoinDAO.GetProductJoins(Context.ProjectId);
            Processes = ProcessDAO.GetAll(Context.ProjectId);
            mainGrid.Background = Brushes.White;
            ProcessGraphBorder.BorderBrush = Brushes.Red;
            this.Content = mainGrid;
            Loaded += MainWindow_Loaded;
            WindowManager = new WindowManager();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            ProcessGraphViewerPanel.ClipToBounds = true;
            ProductGraphViewerPanel.ClipToBounds = true;

            mainGrid.Children.Add(toolBar);
            //DockPanel.SetDock(toolBar, Dock.Top);
            toolBar.VerticalAlignment = VerticalAlignment.Top;

            SetupToolbar();

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
            AddProductNodesInProductGraph();
            AddProductJoinNodes();
            AddProcessNodes();
            AddProductNodesInProcessGraph();
            ProductGraph.Attr.LayerDirection = LayerDirection.RL;
            ProductGraphViewer.Graph = ProductGraph;
            ProcessGraph.Attr.LayerDirection = LayerDirection.RL;
            ProcessGraphViewer.Graph = ProcessGraph;

        }

        private void AddProductNodesInProductGraph()
        {
            foreach (Product product in Products)
            {
                ProductGraph.AddNode(product.Name);
            }
        }

        private void AddProductNodesInProcessGraph()
        {
            foreach (Product product in Products)
            {
                ProcessGraph.AddNode(product.Name);
                ProcessGraph.AddEdge(product.Name, GetProcessById(product.AssociatedProcessId).Name);
            }
        }

        private void AddProductJoinNodes()
        {
            foreach (String productJoinName in ProductJoinNames)
            {
                ProductGraph.AddNode(productJoinName);
                //ProductGraph.AddEdge(GetProductById(productjoin.ChildProductId).Name, productjoin.Name);
            }
        }

        private void AddProcessNodes()
        {
            foreach (Process process in Processes)
            {
                ProcessGraph.AddNode(process.Name);
            }
        }

        private Product GetProductById(int Id)
        {
            /*Product selectedProduct = new Product();
            foreach (Product product in Products)
            {
                if (product.Id == Id)
                {
                    selectedProduct = product;
                    break;
                }
            }
            return selectedProduct;*/
            return new Product();
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

        void SetupToolbar()
        {
            Button addProcessButton = new Button();
            addProcessButton.Content = "Add Process";
            //addProcessButton.Padding = new Thickness(5);
            addProcessButton.FontSize = 14;
            //addProcessButton.Height = 30;
            addProcessButton.Background = Brushes.Transparent;
            addProcessButton.Click += (s, e) => CreateProcess();
            toolBar.Items.Add(addProcessButton);

            Button addProductButton = new Button();
            addProductButton.Content = "Add Product";
            //addProductButton.Padding = new Thickness(5);
            addProductButton.FontSize = 14;
            //addProductButton.Height = 30;
            addProductButton.Background = Brushes.Transparent;
            addProductButton.Click += (s, e) => CreateProduct();
            toolBar.Items.Add(addProductButton);

            Button addProductJoinButton = new Button();
            addProductJoinButton.Content = "Add Product Join";
            //addProductJoinButton.Padding = new Thickness(5);
            addProductJoinButton.FontSize = 14;
            //addProductJoinButton.Height = 30;
            addProductJoinButton.Background = Brushes.Transparent;
            addProductJoinButton.Click += (s, e) => CreateProductJoin();
            toolBar.Items.Add(addProductJoinButton);
        }

        private void CreateProcess()
        {
            ProcessDefinitionView processDefinitionView = new ProcessDefinitionView();
            processDefinitionView.ShowDialog();
            UpdateCollections();
            ProcessGraphViewer.Graph = null;
            ProcessGraph = new Graph();
            AddProcessNodes();
            AddProductNodesInProcessGraph();
            ProcessGraph.Attr.LayerDirection = LayerDirection.RL;
            ProcessGraphViewer.Graph = ProcessGraph;
        }

        private void CreateProduct()
        {
            ProductDefinitionView productDefinitionView = new ProductDefinitionView();
            productDefinitionView.ShowDialog();
            UpdateCollections();
            ProcessGraphViewer.Graph = null;
            ProductGraphViewer.Graph = null;
            ProcessGraph = new Graph();
            ProductGraph = new Graph();
            AddProductNodesInProductGraph();
            AddProductJoinNodes();
            AddProcessNodes();
            AddProductNodesInProcessGraph();
            ProductGraph.Attr.LayerDirection = LayerDirection.RL;
            ProcessGraph.Attr.LayerDirection = LayerDirection.RL;
            ProcessGraphViewer.Graph = ProcessGraph;
            ProductGraphViewer.Graph = ProductGraph;
        }

        private void CreateProductJoin()
        {
            ProductJoinDefinitionView productJoinDefinitionView = new ProductJoinDefinitionView();
            productJoinDefinitionView.ShowDialog();
            UpdateCollections();
            ProductGraphViewer.Graph = null;
            ProductGraph = new Graph();
            AddProductNodesInProductGraph();
            AddProductJoinNodes();
            ProductGraph.Attr.LayerDirection = LayerDirection.RL;
            ProductGraphViewer.Graph = ProductGraph;

        }

        private void UpdateCollections()
        {
            //Processes = ProcessDAO.GetAll(Context.ProjectId);
            //Products = ProductDAO.GetAll(Context.ProjectId);
            //ProductJoins = ProductJoinDAO.GetAll(Context.ProjectId);
        }
    }
}
