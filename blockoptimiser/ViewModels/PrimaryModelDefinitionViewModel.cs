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
        public BindableCollection<FieldMappingModel> FieldMappings { get; set; }
        public BindableCollection<FixedFieldMappingModel> FixedFieldMappings { get; set; }
        public BindableCollection<DimensionModel> DimensionModels { get; set; }
        private String _inputFileName;
        public String ModelBearing { get; set; }
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
            FixedFieldMappings = new BindableCollection<FixedFieldMappingModel> {
                new FixedFieldMappingModel("y", "y", CSVFields),
                new FixedFieldMappingModel("y", "y", CSVFields),
                new FixedFieldMappingModel("z", "z", CSVFields),
                new FixedFieldMappingModel("tonnage", "tonnes_wt", CSVFields)
            };
            FieldMappings = new BindableCollection<FieldMappingModel> {
                new FieldMappingModel("x", "group by", "", CSVFields, DataTypes),
                new FieldMappingModel("y", "group by", "", CSVFields, DataTypes),
                new FieldMappingModel("z", "group by", "", CSVFields, DataTypes),
                new FieldMappingModel("Tonnage", "additive", "", CSVFields, DataTypes),
                new FieldMappingModel("tonnes_wt", "additive", "", CSVFields, DataTypes),
                new FieldMappingModel("mpdest", "group by", "", CSVFields, DataTypes),
                new FieldMappingModel("sprod1_t", "additive", "", CSVFields, DataTypes),
                new FieldMappingModel("sprod1_fe", "grade", "sprod1_t", CSVFields, DataTypes),
            };

            DimensionModels = new BindableCollection<DimensionModel> {
                new DimensionModel("Origin", "1", "2", "3"),
                new DimensionModel("Dimensions", "3", "2", "1"),
                new DimensionModel("Number of Blocks", "1000", "2000", "3000")
            };
        }

        public class FixedFieldMappingModel
        {
            public string FieldName { get; set; }
            public string MapName { get; set; }
            public List<string> MappingOptions { get; set; }
            public FixedFieldMappingModel(string fieldName, string mapName, List<string> mappingOptions)
            {
                FieldName = fieldName;
                MapName = mapName;
                MappingOptions = mappingOptions;
            }
        }

        public class FieldMappingModel
        {
            public string FieldName { get; set; }
            public string DataType { get; set; }
            public string AssociatedField { get; set; }
            public List<string> DataTypeOptions { get; set; }
            public List<string> MappingOptions { get; set; }
            public FieldMappingModel(string fieldName, string dataType, string associatedField, List<string> mappingOptions, List<string> dataTypeOptions)
            {
                FieldName = fieldName;
                DataType = dataType;
                AssociatedField = associatedField;
                MappingOptions = mappingOptions;
                DataTypeOptions = dataTypeOptions;
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
            Console.WriteLine("Inside fetch headers");
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
