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
        ContextMenu contextMenu;
        private readonly IEventAggregator _eventAggregator;
        public V2ProductJoinGraph(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            this.Content = Panel;
            ProductDAO = new ProductDataAccess();
            ProductJoinDAO = new ProductJoinDataAccess();
            Products = ProductDAO.GetAll(Context.ProjectId);
            ProductJoinNames = ProductJoinDAO.GetProductJoins(Context.ProjectId);
            contextMenu = new ContextMenu();
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
            BindContextMenu();
        }

        private void BindContextMenu()
        {
            Panel.ContextMenu = contextMenu;
            System.Windows.Controls.MenuItem addProductJoinMenu = new System.Windows.Controls.MenuItem();
            addProductJoinMenu.Header = "Add Product Join";
            addProductJoinMenu.Click += (s, newevent) => CreateProductJoin();
            System.Windows.Controls.MenuItem editMenu = new System.Windows.Controls.MenuItem();
            editMenu.Header = "Edit";
            editMenu.Click += (s, newevent) => Graph_EditClick();
            System.Windows.Controls.MenuItem deleteMenu = new System.Windows.Controls.MenuItem();
            deleteMenu.Header = "Delete";
            deleteMenu.Click += (s, newevent) => Graph_DeleteClick();
            contextMenu.Items.Add(addProductJoinMenu);
            contextMenu.Items.Add(editMenu);
            contextMenu.Items.Add(deleteMenu);
        }

        private void UpdateCollections()
        {
            Products = ProductDAO.GetAll(Context.ProjectId);
            ProductJoinNames = ProductJoinDAO.GetProductJoins(Context.ProjectId);
        }

        private void Graph_EditClick()
        {
            List<Microsoft.Msagl.WpfGraphControl.VNode> nodesToEdit = new List<Microsoft.Msagl.WpfGraphControl.VNode>();
            foreach (var en in graphViewer.Entities)
            {
                if (en.MarkedForDragging && en is IViewerNode)
                { //got a selected node
                    Microsoft.Msagl.WpfGraphControl.VNode ViewerNode = (Microsoft.Msagl.WpfGraphControl.VNode)en;
                    nodesToEdit.Add(ViewerNode);
                }
            }
            if (nodesToEdit.Count < 1)
            {
                MessageBox.Show("Select atleast one node to delete.");
                return;
            }
            if (nodesToEdit.Count > 1)
            {
                MessageBox.Show("Cannot edit multiple nodes. Select only one.");
                return;
            }
            if (nodesToEdit[0].Node.InEdges.Count() > 0)
            {
                EditProductJoin(nodesToEdit[0].Node.LabelText);
            }
            else {
                Product product = GetProductByName(nodesToEdit[0].Node.LabelText);
                if (product != null) EditProduct(product);
            }
        }

        private void Graph_DeleteClick()
        {
            List<Microsoft.Msagl.WpfGraphControl.VNode> nodesToDelete = new List<Microsoft.Msagl.WpfGraphControl.VNode>();
            foreach (var en in graphViewer.Entities)
            {
                if (en.MarkedForDragging && en is IViewerNode) //got a selected node
                {
                    Microsoft.Msagl.WpfGraphControl.VNode ViewerNode = (Microsoft.Msagl.WpfGraphControl.VNode)en;
                    nodesToDelete.Add(ViewerNode);
                }
            }
            if (nodesToDelete.Count < 1)
            {
                MessageBox.Show("Select atleast one node to delete.");
                return;
            }
            foreach (var node in nodesToDelete)
            {
                if (node.Node.OutEdges.Count() > 0)
                {
                    Product product = GetProductByName(node.Node.LabelText);
                    if (product != null) DeleteProduct(product);

                }
                else
                {
                    DeleteProductJoin(node.Node.LabelText);
                    
                }
            }
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

        private void CreateProductJoin()
        {
            ProductJoinDefinitionView productJoinDefinitionView = new ProductJoinDefinitionView();
            productJoinDefinitionView.ShowDialog();
            UpdateCollections();
            graphViewer.Graph = null;
            graph = new Graph();
            AddProductNodes();
            AddProductJoinNodes();
            graph.Attr.LayerDirection = LayerDirection.TB;
            graphViewer.Graph = graph;

        }

        private void DeleteProduct(Product product)
        {
            ProductDAO.Delete(product);
            UpdateCollections();
            graphViewer.Graph = null;
            graph = new Graph();
            AddProductNodes();
            AddProductJoinNodes();
            graph.Attr.LayerDirection = LayerDirection.TB;
            graphViewer.Graph = graph;
            _eventAggregator.PublishOnUIThread("changed:productJoinGraph");
        }
        private void DeleteProductJoin(String name)
        {
            ProductJoinDAO.Delete(name);
            UpdateCollections();
            graphViewer.Graph = null;
            graph = new Graph();
            AddProductNodes();
            AddProductJoinNodes();
            graph.Attr.LayerDirection = LayerDirection.TB;
            graphViewer.Graph = graph;
        }

        private void EditProduct(Product product)
        {
            ProductEditView productEditView = new ProductEditView(product);
            productEditView.ShowDialog();
            UpdateCollections();
            graphViewer.Graph = null;
            graph = new Graph();
            AddProductNodes();
            AddProductJoinNodes();
            graph.Attr.LayerDirection = LayerDirection.TB;
            graphViewer.Graph = graph;
            _eventAggregator.PublishOnUIThread("changed:productJoinGraph");
        }

        private void EditProductJoin(String productJoin)
        {
            ProductJoinEditView productJoinEditView = new ProductJoinEditView(productJoin);
            productJoinEditView.ShowDialog();
            UpdateCollections();
            graphViewer.Graph = null;
            graph = new Graph();
            AddProductNodes();
            AddProductJoinNodes();
            graph.Attr.LayerDirection = LayerDirection.TB;
            graphViewer.Graph = graph;
        }

        private Product GetProductByName(String name)
        {
            Product returnedProduct = null;
            foreach (Product product in Products)
            {
                if (product.Name == name)
                {
                    returnedProduct = product;
                    break;
                }
            }
            return returnedProduct;
        }
    }
}
