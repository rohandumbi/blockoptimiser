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
    /// Interaction logic for ProcessDefinitionView.xaml
    /// </summary>
    public partial class ProcessDefinitionView : Window
    {
        private ProcessDataAccess ProcessDAO;
        private FieldDataAccess FieldDAO;
        private List<Field> Fields;
        private List<String> FieldNames;
        private String SelectedFieldName;
        public ProcessDefinitionView()
        {
            InitializeComponent();
            ProcessDAO = new ProcessDataAccess();
            FieldDAO = new FieldDataAccess();
            Fields = FieldDAO.GetAll(Context.ProjectId);
            FieldNames = new List<string>();
            foreach (Field field in Fields)
            {
                FieldNames.Add(field.Name);
            }
            BindDropDown();
        }

        private void Field_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedFieldName = fieldCombo.Text;
        }

        private void Field_TextChanged(object sender, TextChangedEventArgs e)
        {
            fieldCombo.ItemsSource = FieldNames.Where(x => x.StartsWith(fieldCombo.Text.Trim()));
        }
        private void BindDropDown()
        {
            fieldCombo.ItemsSource = FieldNames;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            String ProcessName = Name.Text;
            if (ProcessName == null)
            {
                MessageBox.Show("Enter manadatory field NAME");
                return;
            }
            Field SelectedField = GetFieldByName(fieldCombo.Text);
            if (SelectedField.Id == 0)
            {
                MessageBox.Show("Please select a valid FIELD");
                return;
            }
            Process newProcess = new Process();
            newProcess.ProjectId = Context.ProjectId;
            newProcess.Name = ProcessName;
            newProcess.Mapping = new List<ProcessModelMapping>();
            //newProcess.FilterString = FilterString.Text;
            //newProcess.FieldId = SelectedField.Id;
            ProcessDAO.Insert(newProcess);
            this.Close();
        }

        private Field GetFieldByName(string fieldName)
        {
            Field returnedField = new Field();
            foreach (Field field in Fields)
            {
                if (field.Name == fieldName)
                {
                    returnedField = field;
                }
            }
            return returnedField;
        }
    }
}
