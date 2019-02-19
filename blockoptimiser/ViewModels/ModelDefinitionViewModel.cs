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

namespace blockoptimiser.ViewModels
{
    public class ModelDefinitionViewModel : Screen
    {
        private String _inputFileName;
        private CSVReader _fileReader;
        private Model _model;
        private Model _primaryModel;
        private ModelDataAccess _modelDAO;
        private ModelDimensionDataAccess _modelDimensionDAO;
        private CsvColumnMappingDataAccess _csvColumnMappingDAO;
        private Dictionary<String, int> _columnFieldIdMapping;
        public List<String> CSVFields { get; set; }
        public BindableCollection<CSVFieldMapping> CSVFieldMappings { get; set; }
        public BindableCollection<ModelDimension> ModelDimensions { get; set; }

        public Decimal ModelBearing
        {
            get { return _model.Bearing; }
            set
            {
                if (value < 0 || value > 360)
                {
                    MessageBox.Show("Please enter a valid value for bearing.");
                    return;
                }
                _model.Bearing = value;
            }
        } 
        public String InputFile
        {
            get { return _inputFileName; }
            set
            {
                _inputFileName = value;
                Console.WriteLine("Input file name is " + _inputFileName);
                _fileReader = new CSVReader(_inputFileName);
                _fileReader.AddVirtualColumn("Id", "-1");
                CSVFields = new List<string>();
                for(int i = 1; i< _fileReader.Header.Length; i++)
                {
                    CSVFields.Add(_fileReader.Header[i]);
                }
                int count = 0;
                foreach(CSVFieldMapping mapping in CSVFieldMappings)
                {
                    mapping.CSVFields = CSVFields;
                    mapping.ColumnName = CSVFields.ElementAt(count);
                    count++;
                }
                NotifyOfPropertyChange("CSVFieldMappings");
            }
        }
        public ModelDefinitionViewModel()
        {           
            _modelDAO = new ModelDataAccess();
            _modelDimensionDAO = new ModelDimensionDataAccess();
            _csvColumnMappingDAO = new CsvColumnMappingDataAccess();
            _model = _modelDAO.Get(Context.ModelId);
            List<Model> Models = _modelDAO.GetAll(Context.ProjectId);
            ModelDimensions = new BindableCollection<ModelDimension>(_modelDimensionDAO.GetAll(Context.ModelId));
            foreach (var ModelDimension in ModelDimensions)
            {
                ModelDimension.PropertyChanged += ModelDimension_PropertyChanged;
            }

            _primaryModel = Models.ElementAt(0);
            List<CsvColumnMapping> _primaryModelColumns = _csvColumnMappingDAO.GetAll(_primaryModel.Id);
            List<CsvColumnMapping> _csvColumns = _csvColumnMappingDAO.GetAll(Context.ModelId);
            CSVFieldMappings = new BindableCollection<CSVFieldMapping>();
            _columnFieldIdMapping = new Dictionary<string, int>();
            /* 
             * temp fix by Rohan on Arpan's code. Needs verification.
             */
            CSVFields = new List<string>();
            foreach (CsvColumnMapping csvColumn in _csvColumns)
            {
                CSVFields.Add(csvColumn.ColumnName);
            }
            foreach (CsvColumnMapping _primaryModelCsvColumn in _primaryModelColumns)
            {
                _columnFieldIdMapping.Add(_primaryModelCsvColumn.ColumnName, _primaryModelCsvColumn.FieldId);
                CSVFieldMapping mapping = new CSVFieldMapping
                {
                    PrimayModelColumnName = _primaryModelCsvColumn.ColumnName
                };
                foreach (CsvColumnMapping _csvColumn in _csvColumns)
                {
                    if(_csvColumn.FieldId == _primaryModelCsvColumn.FieldId)
                    {
                        mapping.ColumnName = _csvColumn.ColumnName;
                        mapping.DefaultValue = _csvColumn.DefaultValue;
                        break;
                    }
                }
                mapping.CSVFields = CSVFields;
                CSVFieldMappings.Add(mapping);
            }
            foreach (var CSVFieldMapping in CSVFieldMappings)
            {
                CSVFieldMapping.PropertyChanged += CSVFieldMapping_PropertyChanged;
            }
        }

        private void ModelDimension_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Do stuff with fleet here
            ModelDimension modelDimension = (ModelDimension)sender;
            _modelDimensionDAO.Update(modelDimension);
            NotifyOfPropertyChange(() => ModelDimensions);
        }

        private void CSVFieldMapping_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Do stuff with fleet here
            CSVFieldMapping updatedMapping = (CSVFieldMapping)sender;

            _csvColumnMappingDAO.UpdateByColumnName(_columnFieldIdMapping[updatedMapping.PrimayModelColumnName], updatedMapping.DefaultValue, updatedMapping.ColumnName, Context.ModelId);
            NotifyOfPropertyChange(() => CSVFieldMappings);
        }

        public void ImportData()
        {
            if (String.IsNullOrEmpty(_inputFileName))
            {
                MessageBox.Show("Please select a file!");
                return;
            }
            _csvColumnMappingDAO.DeleteAll(Context.ModelId);
            foreach(var CSVFieldMapping in CSVFieldMappings)
            {
                int fieldId = 0;
                if (_columnFieldIdMapping.ContainsKey(CSVFieldMapping.PrimayModelColumnName))
                {
                    fieldId = _columnFieldIdMapping[CSVFieldMapping.PrimayModelColumnName];
                }
                CsvColumnMapping csvColumnMapping = new CsvColumnMapping
                {
                    ModelId = Context.ModelId,
                    ColumnName = CSVFieldMapping.ColumnName,
                    FieldId = fieldId,
                    DefaultValue = CSVFieldMapping.DefaultValue
                };
                _csvColumnMappingDAO.Insert(csvColumnMapping);
            }
            List<RequiredFieldMapping> RequiredFieldMappings = new RequiredFieldMappingDataAccess().GetAll(Context.ProjectId);
            Dictionary<String, String> FixedFieldCsvMapping = new Dictionary<string, string>();
            foreach (RequiredFieldMapping mapping in RequiredFieldMappings)
            { 
                String mappedColumn = "";
                foreach(var CSVFieldMapping in CSVFieldMappings)
                {
                    if(CSVFieldMapping.PrimayModelColumnName == mapping.MappedColumnName)
                    {
                        mappedColumn = CSVFieldMapping.ColumnName;
                        break;
                    }
                }
                FixedFieldCsvMapping.Add(mapping.RequiredFieldName, mappedColumn);
            }
            decimal angle = ModelBearing;
            if (ModelBearing > 90 && ModelBearing < 180)
            {
                angle = ModelBearing - 90;
            }
            else if (ModelBearing > 180 && ModelBearing < 270)
            {
                angle = ModelBearing - 180;
            }
            else if (ModelBearing > 270 && ModelBearing < 360)
            {
                angle = ModelBearing - 270;
            }

            CSVDataLoader loader = new CSVDataLoader(_fileReader);
            loader.Load();
            loader.LoadComputedDataTable(FixedFieldCsvMapping, ModelDimensions.ToList(), angle);
            _model.HasData = true;
            _modelDAO.Update(_model);
            MessageBox.Show("File imported successfully.");
        }
    }

    public class CSVFieldMapping : INotifyPropertyChanged
    {
        private String _columnName;
        public String PrimayModelColumnName { get; set; }
        public String ColumnName {
            get {
                return _columnName;
            }
            set {
                _columnName = value;
                OnPropertyChanged("ColumnName");
            }
        }
        public String DefaultValue { get; set; }
        public List<String> CSVFields { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
