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
        public BindableCollection<string> FixedFields { get; set; }
        public List<string> CSVFields { get; set; }
        public BindableCollection<PrimaryFieldMappingModel> PrimaryFieldMappings { get; set; }
        public BindableCollection<DimensionModel> DimensionModels { get; set; }
        private String _fleetFileName;
        public String ModelBearing { get; set; }
        public ModelDefinitionViewModel()
        {
            CSVFields = new List<string>
            {
                "x", "y", "z", "Tonnage", "tonnes_wt", "mpdest", "sprod1_t", "sprod1_fe"
            };
            PrimaryFieldMappings = new BindableCollection<PrimaryFieldMappingModel> {
                new PrimaryFieldMappingModel("y", "y", CSVFields),
                new PrimaryFieldMappingModel("y", "y", CSVFields),
                new PrimaryFieldMappingModel("z", "z", CSVFields),
                new PrimaryFieldMappingModel("tonnage", "tonnage", CSVFields),
                new PrimaryFieldMappingModel("tonnes_wt", "tonnes_wt", CSVFields),
                new PrimaryFieldMappingModel("mpdest", "mpdest", CSVFields),
                new PrimaryFieldMappingModel("sprod1_t", "sprod1_t", CSVFields),
                new PrimaryFieldMappingModel("sprod1_fe", "sprod1_fe", CSVFields),
            };

            DimensionModels = new BindableCollection<DimensionModel> {
                new DimensionModel("Origin", "1", "2", "3"),
                new DimensionModel("Dimensions", "3", "2", "1"),
                new DimensionModel("Number of Blocks", "1000", "2000", "3000")
            };
        }

        public class PrimaryFieldMappingModel
        {
            public string FieldName { get; set; }
            public string MapName { get; set; }
            public List<string> MappingOptions { get; set; }
            public PrimaryFieldMappingModel(string fieldName, string mapName, List<string> mappingOptions)
            {
                FieldName = fieldName;
                MapName = mapName;
                MappingOptions = mappingOptions;
            }
        }



        public class DimensionModel
        {
            public string Name { get; set; }
            public string XCordinate { get; set; }
            public string YCordinate { get; set; }
            public string ZCordinate { get; set; }

            public DimensionModel(string name, string xCordinate, string yCordinate, string zCordinate)
            {
                Name = name;
                XCordinate = xCordinate;
                YCordinate = yCordinate;
                ZCordinate = zCordinate;
            }

        }

        public String FleetFile
        {
            set { _fleetFileName = value; }
        }

        public void ImportData()
        {
            if (String.IsNullOrEmpty(_fleetFileName))
            {
                MessageBox.Show("Please select a file!");
                return;
            }
            if (String.IsNullOrEmpty(ModelBearing))
            {
                MessageBox.Show("Please provide a value for model bearing!");
                return;
            }
            MessageBox.Show("File selected is: " + _fleetFileName + " and model bearing is: " + ModelBearing);
        }
    }
}
