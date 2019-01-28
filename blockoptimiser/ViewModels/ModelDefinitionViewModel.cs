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
    public class ModelDefinitionViewModel : Screen
    {
        public List<string> CSVFields { get; set; }
        public BindableCollection<ModelDimension> ModelDimensions { get; set; }
        private String _inputFileName;
        public String ModelBearing { get; set; }
        public ModelDefinitionViewModel()
        {
            CSVFields = new List<string>
            {
                "x", "y", "z", "Tonnage", "tonnes_wt", "mpdest", "sprod1_t", "sprod1_fe"
            };
            

            ModelDimensions = new BindableCollection<ModelDimension>(new ModelDimensionDataAccess().GetAll(1));
        }
 
        public String InputFile
        {
            set {
                _inputFileName = value;
                Console.WriteLine("Inside model definition view model");
            }
        }

        public void ImportData()
        {
            if (String.IsNullOrEmpty(_inputFileName))
            {
                MessageBox.Show("Please select a file!");
                return;
            }
            if (String.IsNullOrEmpty(ModelBearing))
            {
                MessageBox.Show("Please provide a value for model bearing!");
                return;
            }
            MessageBox.Show("File selected is: " + _inputFileName + " and model bearing is: " + ModelBearing);
        }
    }
}
