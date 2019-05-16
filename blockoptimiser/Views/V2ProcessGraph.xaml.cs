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
        ContextMenu contextMenu;
        private readonly IEventAggregator _eventAggregator;
        public V2ProcessGraph(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            this.Content = Panel;
            ProcessDAO = new ProcessDataAccess();
            ProductDAO = new ProductDataAccess();
            Processes = ProcessDAO.GetAll(Context.ProjectId);
            Products = ProductDAO.GetAll(Context.ProjectId);
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
            AddProcessNodes();
            AddProductNodes();
            graphViewer.Graph = graph; // throws exception
            BindContextMenu();
        }

        private void BindContextMenu()
        {
            Panel.ContextMenu = contextMenu;
            System.Windows.Controls.MenuItem addProcessMenu = new System.Windows.Controls.MenuItem();
            addProcessMenu.Header = "Add Process";
            addProcessMenu.Click += (s, newevent) => CreateProcess();
            System.Windows.Controls.MenuItem addProductMenu = new System.Windows.Controls.MenuItem();
            addProductMenu.Header = "Add Product";
            addProductMenu.Click += (s, newevent) => CreateProduct();
            System.Windows.Controls.MenuItem editMenu = new System.Windows.Controls.MenuItem();
            editMenu.Header = "Edit";
            editMenu.Click += (s, newevent) => Graph_EditClick();
            System.Windows.Controls.MenuItem deleteMenu = new System.Windows.Controls.MenuItem();
            deleteMenu.Header = "Delete";
            deleteMenu.Click += (s, newevent) => Graph_DeleteClick();
            contextMenu.Items.Add(addProcessMenu);
            contextMenu.Items.Add(addProductMenu);
            contextMenu.Items.Add(editMenu);
            contextMenu.Items.Add(deleteMenu);
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
            if (nodesToEdit[0].Node.OutEdges.Count() > 0)
            {
                Product product = GetProductByName(nodesToEdit[0].Node.LabelText);
                if (product != null) EditProduct(product);
            }
            else
            {
                Process process = GetProcessByName(nodesToEdit[0].Node.LabelText);
                if (process != null) EditProcess(process);
            }
        }

        private void Graph_DeleteClick()
        {
            List<Microsoft.Msagl.WpfGraphControl.VNode> nodesToDelete = new List<Microsoft.Msagl.WpfGraphControl.VNode>();
            foreach (var en in graphViewer.Entities)
            {
                if (en.MarkedForDragging && en is IViewerNode)
                { //got a selected node}
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
                    Process process = GetProcessByName(node.Node.LabelText);
                    if (process != null) DeleteProcess(process);
                }
            }
        }

        private void UpdateCollections()
        {
            Processes = ProcessDAO.GetAll(Context.ProjectId);
            Products = ProductDAO.GetAll(Context.ProjectId);
        }

        private void CreateProcess()
        {
            ProcessDefinitionView processDefinitionView = new ProcessDefinitionView();
            processDefinitionView.ShowDialog();
            UpdateCollections();
            graphViewer.Graph = null;
            graph = new Graph();
            AddProcessNodes();
            AddProductNodes();
            graph.Attr.LayerDirection = LayerDirection.TB;
            graphViewer.Graph = graph;
        }

        private void CreateProduct()
        {
            ProductDefinitionView productDefinitionView = new ProductDefinitionView();
            productDefinitionView.ShowDialog();
            UpdateCollections();
            graphViewer.Graph = null;
            graph = new Graph();
            AddProcessNodes();
            AddProductNodes();
            graph.Attr.LayerDirection = LayerDirection.TB;
            graphViewer.Graph = graph;
            _eventAggregator.PublishOnUIThread("changed:processGraph");
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

        private Process GetProcessByName(String name)
        {
            Process returnedProcess = null;
            foreach (Process process in Processes)
            {
                if (process.Name == name)
                {
                    returnedProcess = process;
                    break;
                }
            }
            return returnedProcess;
        }

        private void DeleteProcess(Process process)
        {
            ProcessDAO.Delete(process.Id);
            UpdateCollections();
            graphViewer.Graph = null;
            graph = new Graph();
            AddProcessNodes();
            AddProductNodes();
            graph.Attr.LayerDirection = LayerDirection.TB;
            graphViewer.Graph = graph;
        }

        private void DeleteProduct(Product product)
        {
            ProductDAO.Delete(product);
            UpdateCollections();
            graphViewer.Graph = null;
            graph = new Graph();
            AddProcessNodes();
            AddProductNodes();
            graph.Attr.LayerDirection = LayerDirection.TB;
            graphViewer.Graph = graph;
            _eventAggregator.PublishOnUIThread("changed:processGraph");
        }

        private void EditProduct(Product product)
        {
            ProductEditView productEditView = new ProductEditView(product);
            productEditView.ShowDialog();
            UpdateCollections();
            graphViewer.Graph = null;
            graph = new Graph();
            AddProcessNodes();
            AddProductNodes();
            graph.Attr.LayerDirection = LayerDirection.TB; 
            graphViewer.Graph = graph;
        }

        private void EditProcess(Process process)
        {
            ProcessEditView processEditView = new ProcessEditView(process);
            processEditView.ShowDialog();
            UpdateCollections();
            graphViewer.Graph = null;
            graph = new Graph();
            AddProcessNodes();
            AddProductNodes();
            graph.Attr.LayerDirection = LayerDirection.RL;
            graphViewer.Graph = graph;
        }
    }
}
