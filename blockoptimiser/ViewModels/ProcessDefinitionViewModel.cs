using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace blockoptimiser.ViewModels
{
    public class ProcessDefinitionViewModel : Screen
    {
        private FieldDataAccess FieldDAO;
        private ProcessDataAccess ProcessDAO;
        public List<Field> Fields { get; set; }
        public BindableCollection<String> FieldNames { get; set; }
        public String Name { get; set; }
        public String FilterString { get; set; }
        public String SelectedFieldName { get; set; }

        public ProcessDefinitionViewModel()
        {
            FieldDAO = new FieldDataAccess();
            ProcessDAO = new ProcessDataAccess();
            Fields = FieldDAO.GetAll(Context.ProjectId);
            FieldNames = new BindableCollection<String>();

            foreach (Field field in Fields)
            {
                FieldNames.Add(field.Name);
            }
        }

        public void AddProcess()
        {
            //MessageBox.Show("Name:" + Name + "  FilterString:" + FilterString + "  FilterId:" + GetFieldByName(SelectedFieldName).Id);
            if (Name == null)
            {
                MessageBox.Show("Mandatory field NAME missing.");
                return;
            }
            if (SelectedFieldName == null)
            {
                MessageBox.Show("Mandatory field FIELD missing.");
                return;
            }
            Process newProcess = new Process();
            newProcess.ProjectId = Context.ProjectId;
            newProcess.Name = Name;
            newProcess.FieldId = GetFieldByName(SelectedFieldName).Id;
            newProcess.FilterString = FilterString;
            ProcessDAO.Insert(newProcess);
        }

        private Field GetFieldByName(string fieldName)
        {
            Field returnedField = Fields.First();
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
