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


        private FieldDataAccess FieldDAO;
        private List<Field> Fields;
        private ExpressionDataAccess ExpressionDAO;
        private List<Models.Expression> Expressions;

        public List<UnitItem> UnitItems { get; set; }
        public UnitItem SelectedUnit { get; set; }

        public ProductDefinitionView()
        {
            InitializeComponent();
            ProcessDAO = new ProcessDataAccess();
            ProductDAO = new ProductDataAccess();
            FieldDAO = new FieldDataAccess();
            ExpressionDAO = new ExpressionDataAccess();
            Processes = ProcessDAO.GetAll(Context.ProjectId);
            Fields = FieldDAO.GetAll(Context.ProjectId);
            Expressions = ExpressionDAO.GetAll(Context.ProjectId);
            UnitItems = new List<UnitItem>();

            foreach (Field field in Fields)
            {
                UnitItems.Add(new UnitItem(field.Name, field.Id, Product.UNIT_TYPE_FIELD));
            }

            foreach (Models.Expression expression in Expressions)
            {
                UnitItems.Add(new UnitItem(expression.Name, expression.Id, Product.UNIT_TYPE_EXPRESSION));
            }

            BindDropDown();
        }

        private void BindDropDown()
        {
            processCombo.ItemsSource = Processes;
            fieldsCombo.ItemsSource = UnitItems;
        }
        private void Process_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Field_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox fieldsCombo = (ComboBox)sender;
            SelectedUnit = (UnitItem)fieldsCombo.SelectedItem;
        }

        private void Process_TextChanged(object sender, TextChangedEventArgs e)
        {
            processCombo.ItemsSource = Processes.Where(x => x.Name.StartsWith(processCombo.Text.Trim()));
        }

        private void AllCheckbocx_CheckedAndUnchecked(object sender, RoutedEventArgs e)
        {
            BindListBOX();
        }

        private void BindListBOX()
        {
            testListbox.Items.Clear();
            foreach (var process in Processes)
            {
                if (process.CheckStatus == true)
                {
                    testListbox.Items.Add(process.Name);
                }
            }
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            String ProductName = Name.Text;
            if (ProductName == null)
            {
                MessageBox.Show("Enter manadatory field NAME");
                return;
            }
            List<int> selectedProcessIds = new List<int>();
            foreach (Process process in Processes)
            {
                if (process.CheckStatus == true)
                {
                    selectedProcessIds.Add(process.Id);
                }
            }
            //Process selectedProcess = GetProcessByName(processCombo.Text);
            if (selectedProcessIds.Count == 0)
            {
                MessageBox.Show("Select a valid PROCESS");
                return;
            }
            Product newProduct = new Product();
            newProduct.Name = ProductName;
            newProduct.ProjectId = Context.ProjectId;
            newProduct.ProcessIds = selectedProcessIds;
            newProduct.UnitId = SelectedUnit.UnitId;
            newProduct.UnitType = SelectedUnit.UnitType;
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
