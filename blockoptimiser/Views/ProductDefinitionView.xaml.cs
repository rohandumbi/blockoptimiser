using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
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
using System.Windows.Shapes;

namespace blockoptimiser.Views
{
    /// <summary>
    /// Interaction logic for ProductDefinitionView.xaml
    /// </summary>
    public partial class ProductDefinitionView : Window
    {
        private ProcessDataAccess ProcessDAO;
        private ProductDataAccess ProductDAO;
        public List<Process> Processes { get; set; }
        public List<String> ProcessNames { get; set; }
        private String SelectedProcessName;

        public ProductDefinitionView()
        {
            InitializeComponent();
            ProcessDAO = new ProcessDataAccess();
            ProductDAO = new ProductDataAccess();
            Processes = ProcessDAO.GetAll(Context.ProjectId);
            ProcessNames = new List<string>();
            foreach (Process process in Processes)
            {
                ProcessNames.Add(process.Name);
            }
            BindDropDown();
        }

        private void Process_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedProcessName = processCombo.Text;
        }

        private void Process_TextChanged(object sender, TextChangedEventArgs e)
        {
            processCombo.ItemsSource = ProcessNames.Where(x => x.StartsWith(processCombo.Text.Trim()));
        }
        private void BindDropDown()
        {
            processCombo.ItemsSource = ProcessNames;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            String ProductName = Name.Text;
            if (ProductName == null)
            {
                MessageBox.Show("Enter manadatory field NAME");
                return;
            }
            Process selectedProcess = GetProcessByName(processCombo.Text);
            if (selectedProcess.Id == 0)
            {
                MessageBox.Show("Select a valid PROCESS");
                return;
            }
            Product newProduct = new Product();
            newProduct.Name = ProductName;
            newProduct.ProjectId = Context.ProjectId;
            newProduct.ProcessIds = new List<int>();
            newProduct.ProcessIds.Add(selectedProcess.Id);
            ProductDAO.Insert(newProduct);
            this.Close();
        }

        private Process GetProcessByName(String processName)
        {
            Process returnedProcess = new Process();
            foreach (Process process in Processes)
            {
                if (process.Name == processName)
                {
                    returnedProcess = process;
                    break;
                }
            }
            return returnedProcess;
        }
    }
}
