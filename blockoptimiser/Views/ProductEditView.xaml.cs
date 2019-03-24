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
    public partial class ProductEditView : Window
    {
        private ProcessDataAccess ProcessDAO;
        private ProductDataAccess ProductDAO;
        private ModelDataAccess ModelDAO;
        private CsvColumnMappingDataAccess CsvDAO;
        public List<Process> Processes { get; set; }


        private FieldDataAccess FieldDAO;
        private List<Field> Fields;
        private ExpressionDataAccess ExpressionDAO;
        private List<Models.Expression> Expressions;

        public List<UnitItem> UnitItems { get; set; }
        public UnitItem SelectedUnit { get; set; }

        public List<Model> Models;
        private Product UpdatedProduct;

        public ProductEditView(Product product)
        {
            InitializeComponent();
            UpdatedProduct = product;
            ProcessDAO = new ProcessDataAccess();
            ProductDAO = new ProductDataAccess();
            FieldDAO = new FieldDataAccess();
            ModelDAO = new ModelDataAccess();
            ExpressionDAO = new ExpressionDataAccess();
            CsvDAO = new CsvColumnMappingDataAccess();
            Processes = ProcessDAO.GetAll(Context.ProjectId);
            Fields = FieldDAO.GetAll(Context.ProjectId);
            Expressions = ExpressionDAO.GetAll(Context.ProjectId);
            Models = ModelDAO.GetAll(Context.ProjectId);
            UnitItems = new List<UnitItem>();

            foreach (Field field in Fields)
            {
                if (field.DataType == Field.DATA_TYPE_ADDITIVE)
                {
                    UnitItems.Add(new UnitItem(field.Name, field.Id, Product.UNIT_TYPE_FIELD));
                }
            }

            foreach (Model model in Models)
            {
                List<CsvColumnMapping> csvColumns = CsvDAO.GetAll(model.Id);
                List<UnitItem> units = new List<UnitItem>();
                foreach (CsvColumnMapping csvColumn in csvColumns)
                {
                    units.Add(new UnitItem(csvColumn.ColumnName, csvColumn.FieldId, Product.UNIT_TYPE_FIELD));
                }
                foreach (Models.Expression expression in Expressions)
                {
                    units.Add(new UnitItem(expression.Name, expression.Id, Product.UNIT_TYPE_EXPRESSION));
                }
                model.AssociatedUnitItems = units;
            }
            Name.Text = UpdatedProduct.Name;
            SelectedUnit = new UnitItem(UpdatedProduct.UnitName, UpdatedProduct.UnitId, UpdatedProduct.UnitType);
            UnitName.Text = SelectedUnit.Name;
            BindProcessDropDown();
            BindFieldDropDown();
            BindListBox();
            //BindAllModels();
        }

        private void BindProcessDropDown()
        {
            foreach (Process process in Processes)
            {
                foreach (int processId in UpdatedProduct.ProcessIds)
                {
                    if (process.Id == processId)
                    {
                        process.CheckStatus = true;
                    }
                }
            }
            processCombo.ItemsSource = Processes;
        }
        private void BindFieldDropDown()
        {
            fieldsCombo.ItemsSource = UnitItems;
            //fieldsCombo.SelectedItem = SelectedUnit;
            //fieldsCombo.SelectedValue = SelectedUnit;
            //fieldsCombo.Text = SelectedUnit.Name;
        }

        private void Field_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox fieldsCombo = (ComboBox)sender;
            SelectedUnit = (UnitItem)fieldsCombo.SelectedItem;
            UnitName.Text = SelectedUnit.Name;
        }

        //private void BindAllModels()
        //{
        //    ModelMapping.ItemsSource = Models;
        //}

        private void Process_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Grade_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }


        private void Process_TextChanged(object sender, TextChangedEventArgs e)
        {
            processCombo.ItemsSource = Processes.Where(x => x.Name.StartsWith(processCombo.Text.Trim()));
        }

        private void Grade_TextChanged(object sender, TextChangedEventArgs e)
        {
            //gradeCombo.ItemsSource = Fields.Where(x => x.Name.StartsWith(gradeCombo.Text.Trim()));
        }

        private void AllCheckbocx_CheckedAndUnchecked(object sender, RoutedEventArgs e)
        {
            BindListBox();
        }

        private void AllCheckbocx_CheckedAndUnchecked_Grade(object sender, RoutedEventArgs e)
        {
            //BindListBOXGrade();
        }

        private void BindListBox()
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
            if (ProductName == null || ProductName == "")
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
            if (selectedProcessIds.Count == 0)
            {
                MessageBox.Show("Select a valid PROCESS");
                return;
            }
            if (selectedProcessIds.Count == 0)
            {
                MessageBox.Show("Select a valid PROCESS");
                return;
            }
            if (SelectedUnit == null)
            {
                MessageBox.Show("Select a valid Field");
                return;
            }
            
            UpdatedProduct.ProcessIds = selectedProcessIds;
            UpdatedProduct.UnitId = SelectedUnit.UnitId;
            UpdatedProduct.UnitName = SelectedUnit.Name;
            //newProduct.GradeNames = selectedGradeNames;

            //ProductDAO.Insert(newProduct);
            try
            {
                ProductDAO.UpdateUnit(UpdatedProduct);
                ProductDAO.UpdateProcessMapping(UpdatedProduct);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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
