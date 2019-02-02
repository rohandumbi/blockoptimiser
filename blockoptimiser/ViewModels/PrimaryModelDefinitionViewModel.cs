using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using blockoptimiser.Services.DataImport;
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
        private String _inputFileName;
        private CSVReader _fileReader;
        public BindableCollection<string> FixedFields { get; set;}
        public String[] CSVFields { get; set; }
        public int[] DataTypes { get; set; }
        public BindableCollection<Field> Fields { get; set; }
        public BindableCollection<CsvColumnMapping> CSVFieldMappings { get; set; }
        public BindableCollection<RequiredFieldMapping> RequiredFieldMappings { get; set; }
        public BindableCollection<ModelDimension> ModelDimensions { get; set; }
        public String ModelBearing { get; set; }

        public PrimaryModelDefinitionViewModel()
        {
            RequiredFieldMappings = new BindableCollection<RequiredFieldMapping>(new RequiredFieldMappingDataAccess().GetAll());
            Fields = new BindableCollection<Field>(new FieldDataAccess().GetAll(Context.ProjectId));
            CSVFieldMappings = new BindableCollection<CsvColumnMapping>(new CsvColumnMappingDataAccess().GetAll(1));
            ModelDimensions = new BindableCollection<ModelDimension>(new ModelDimensionDataAccess().GetAll(Context.ModelId));
        }

        public String InputFile
        {
            set {
                _inputFileName = value;
                Console.WriteLine("Input file name is "+ _inputFileName);
                _fileReader = new CSVReader(_inputFileName, true);
                CSVFields = _fileReader.Header;
                DataTypes = _fileReader.DataTypes;
                Fields = new BindableCollection<Field>();
                Field LastAdditiveField = null ;
                for(int i = 0; i< CSVFields.Length; i++)
                {
                    Field field = new Field
                    {
                        Name = CSVFields[i],
                        DataType = DataTypes[i]
                    };
                    switch (field.DataType)
                    {
                        case Field.DATA_TYPE_GROUP_BY:
                            field.DataTypeName = "groupby";
                            break;
                        case Field.DATA_TYPE_ADDITIVE:
                            field.DataTypeName = "additive";
                            LastAdditiveField = field;
                            break;
                        case Field.DATA_TYPE_GRADE:
                            field.DataTypeName = "grade";
                            field.AssociatedField = LastAdditiveField.Id;
                            field.AssociatedFieldName = LastAdditiveField.Name;
                            break;
                    }
                    Fields.Add(field);
                }
                NotifyOfPropertyChange("Fields");
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
            CSVDataLoader loader = new CSVDataLoader(_fileReader);
            loader.Load();
            MessageBox.Show("File imported successfully.");
        }
    }
}
