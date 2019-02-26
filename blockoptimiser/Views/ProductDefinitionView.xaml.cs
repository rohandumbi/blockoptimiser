﻿using blockoptimiser.DataAccessClasses;
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

        public ProductDefinitionView()
        {
            InitializeComponent();
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
                UnitItems.Add(new UnitItem(field.Name, field.Id, Product.UNIT_TYPE_FIELD));
            }

            foreach (Models.Expression expression in Expressions)
            {
                UnitItems.Add(new UnitItem(expression.Name, expression.Id, Product.UNIT_TYPE_EXPRESSION));
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

            BindDropDown();
            //BindAllModels();
        }

        private void BindDropDown()
        {
            processCombo.ItemsSource = Processes;
            gradeCombo.ItemsSource = Fields;
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
            gradeCombo.ItemsSource = Fields.Where(x => x.Name.StartsWith(gradeCombo.Text.Trim()));
        }

        private void AllCheckbocx_CheckedAndUnchecked(object sender, RoutedEventArgs e)
        {
            BindListBOX();
        }

        private void AllCheckbocx_CheckedAndUnchecked_Grade(object sender, RoutedEventArgs e)
        {
            BindListBOXGrade();
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
        private void BindListBOXGrade()
        {
            gradeListbox.Items.Clear();
            foreach (var field in Fields)
            {
                if (field.CheckStatus == true)
                {
                    gradeListbox.Items.Add(field.Name);
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
            //Process selectedProcess = GetProcessByName(processCombo.Text);
            if (selectedProcessIds.Count == 0)
            {
                MessageBox.Show("Select a valid PROCESS");
                return;
            }

            List<String> selectedGradeNames = new List<String>();
            foreach (Field field in Fields)
            {
                if (field.CheckStatus == true)
                {
                    selectedGradeNames.Add(field.Name);
                }
            }
            //Process selectedProcess = GetProcessByName(processCombo.Text);
            if (selectedProcessIds.Count == 0)
            {
                MessageBox.Show("Select a valid PROCESS");
                return;
            }

            if (selectedGradeNames.Count == 0)
            {
                MessageBox.Show("Select a valid Grade");
                return;
            }

            Product newProduct = new Product();
            newProduct.Name = ProductName;
            newProduct.ProjectId = Context.ProjectId;
            newProduct.ProcessIds = selectedProcessIds;
            newProduct.GradeNames = selectedGradeNames;
            
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
