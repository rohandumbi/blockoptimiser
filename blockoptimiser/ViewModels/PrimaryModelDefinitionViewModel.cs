using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using blockoptimiser.Services.DataImport;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace blockoptimiser.ViewModels
{
    public class PrimaryModelDefinitionViewModel: Screen
    {
        private String _inputFileName;
        private CSVReader _fileReader;
        private Model _model;
        private ModelDataAccess _modelDAO;
        private ModelDimensionDataAccess _modelDimensionDAO;
        private RequiredFieldMappingDataAccess _requiredFieldMappingDAO;
        private FieldDataAccess _fieldDAO;
        private CsvColumnMappingDataAccess _csvColumnMappingDAO;

        public BindableCollection<string> FixedFields { get; set;}
        public List<String> CSVFields { get; set; }
        public int[] DataTypes { get; set; }
        public BindableCollection<Field> Fields { get; set; }
        public BindableCollection<CsvColumnMapping> CSVFieldMappings { get; set; }
        public BindableCollection<RequiredFieldMapping> RequiredFieldMappings { get; set; }
        public BindableCollection<ModelDimension> ModelDimensions { get; set; }

        public Decimal ModelBearing
        {
            get { return _model.Bearing; }
            set {
                if(value < 0 || value > 360)
                {
                    MessageBox.Show("Please enter a valid value for bearing.");
                    return;
                }
                _model.Bearing = value;
            }
        }

        public String InputFile
        {
            set
            {
                _inputFileName = value;
                Console.WriteLine("Input file name is " + _inputFileName);
                _fileReader = new CSVReader(_inputFileName, true);
                DataTypes = _fileReader.DataTypes;
                CSVFields = new List<string>();
                for (int i = 0; i < _fileReader.Header.Length; i++)
                {
                    CSVFields.Add(_fileReader.Header[i]);
                }
                Fields = new BindableCollection<Field>();
                Field LastAdditiveField = null;
                for (int i = 0; i < CSVFields.Count; i++)
                {
                    Field field = new Field
                    {
                        ProjectId = Context.ProjectId,
                        Name = CSVFields.ElementAt(i),
                        DataType = DataTypes[i]
                    };
                    if (field.DataType == Field.DATA_TYPE_ADDITIVE)
                    {
                        LastAdditiveField = field;
                    }
                    else if (field.DataType == Field.DATA_TYPE_GRADE)
                    {
                        field.AssociatedField = LastAdditiveField.Id;
                        field.AssociatedFieldName = LastAdditiveField.Name;
                    }
                    Fields.Add(field);
                }
                foreach (var RequiredFieldMapping in RequiredFieldMappings)
                {
                    RequiredFieldMapping.mappingOptions = CSVFields;
                }
                NotifyOfPropertyChange("RequiredFieldMappings");
                NotifyOfPropertyChange("Fields");
            }
        }

        public PrimaryModelDefinitionViewModel()
        {
            _modelDAO = new ModelDataAccess();
            _modelDimensionDAO = new ModelDimensionDataAccess();
            _fieldDAO = new FieldDataAccess();
            _csvColumnMappingDAO = new CsvColumnMappingDataAccess();
            _requiredFieldMappingDAO = new RequiredFieldMappingDataAccess();
            _model = _modelDAO.Get(Context.ModelId);
            Fields = new BindableCollection<Field>(_fieldDAO.GetAll(Context.ProjectId));
            CSVFieldMappings = new BindableCollection<CsvColumnMapping>(_csvColumnMappingDAO.GetAll(Context.ModelId));
            CSVFields = new List<string>();
            foreach(var CSVFieldMapping in CSVFieldMappings)
            {
                CSVFields.Add(CSVFieldMapping.ColumnName);
            }
            ModelDimensions = new BindableCollection<ModelDimension>(_modelDimensionDAO.GetAll(Context.ModelId));
            foreach(var ModelDimension in ModelDimensions)
            {
                ModelDimension.PropertyChanged += ModelDimension_PropertyChanged;
            }
            RequiredFieldMappings = new BindableCollection<RequiredFieldMapping>(_requiredFieldMappingDAO.GetAll(Context.ProjectId));
            foreach (var RequiredFieldMapping in RequiredFieldMappings)
            {
                RequiredFieldMapping.PropertyChanged += RequiredFieldMapping_PropertyChanged;
                RequiredFieldMapping.mappingOptions = CSVFields;
            }
        }

        private void RequiredFieldMapping_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RequiredFieldMapping requiredFieldMapping = (RequiredFieldMapping)sender;
            _requiredFieldMappingDAO.Update(requiredFieldMapping);
            NotifyOfPropertyChange(() => RequiredFieldMappings);
        }
        private void ModelDimension_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ModelDimension modelDimension = (ModelDimension)sender;
            _modelDimensionDAO.Update(modelDimension);
            NotifyOfPropertyChange(() => ModelDimensions);
        }

        public void ImportData()
        {
            if (!ValidateData()) return;
            // Load all the fields
            _fieldDAO.DeleteAll(Context.ProjectId);
            _csvColumnMappingDAO.DeleteAll(Context.ModelId);
            int count = 0;
            foreach (Field newField in Fields)
            {
                _fieldDAO.Insert(newField);
                CsvColumnMapping _csvColumnMapping = new CsvColumnMapping
                {
                    ModelId = Context.ModelId,
                    ColumnName = CSVFields[count],
                    FieldId = newField.Id
                };
                _csvColumnMappingDAO.Insert(_csvColumnMapping);
                count++;
            }
            foreach (Field newField in Fields)
            {
                if(newField.DataType == Field.DATA_TYPE_GRADE)
                {
                    foreach (Field field in Fields)
                    {
                        if(field.DataType == Field.DATA_TYPE_ADDITIVE && field.Name.Equals(newField.AssociatedFieldName))
                        {
                            newField.AssociatedField = field.Id;
                            _fieldDAO.Update(newField);
                        }
                    }
                }
            }
            CSVDataLoader loader = new CSVDataLoader(_fileReader);
            loader.Load();
            _model.HasData = true;
            _modelDAO.Update(_model);
            MessageBox.Show("File imported successfully.");
        }

        private Boolean ValidateData()
        {
            if (String.IsNullOrEmpty(_inputFileName))
            {
                MessageBox.Show("Please select a file!");
            }

            return true;
        }
    }
}
