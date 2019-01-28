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
    public class PrimaryModelDefinitionViewModel: Screen
    {
        public BindableCollection<string> FixedFields { get; set;}
        public List<string> CSVFields { get; set; }
        public List<string> DataTypes { get; set; }
        public BindableCollection<Field> Fields { get; set; }
        public BindableCollection<RequiredFieldMapping> RequiredFieldMappings { get; set; }
        public BindableCollection<ModelDimension> ModelDimensions { get; set; }
        private String _inputFileName;

        public PrimaryModelDefinitionViewModel()
        {
            CSVFields = new List<string>
            {
                "x", "y", "z", "Tonnage", "tonnes_wt", "mpdest", "sprod1_t", "sprod1_fe"
            };
            DataTypes = new List<string>
            {
                "group by", "additive", "grade"
            };
            RequiredFieldMappings = new BindableCollection<RequiredFieldMapping>(new RequiredFieldMappingDataAccess().GetAll());
            Fields = new BindableCollection<Field>(new FieldDataAccess().GetAll(Context.ProjectId));
            ModelDimensions = new BindableCollection<ModelDimension>(new ModelDimensionDataAccess().GetAll());
        }

        public String InputFile
        {
            set {
                _inputFileName = value;
                Console.WriteLine("Input file name is "+ _inputFileName);
                FetchHeaders();
            }
        }
        private void FetchHeaders()
        {
            
        }
        public void ImportData()
        {
            if (String.IsNullOrEmpty(_inputFileName))
            {
                MessageBox.Show("Please select a file!");
                return;
            }
            MessageBox.Show("File selected is: " + _inputFileName);
        }
    }
}
