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
    public class ModelDefinitionViewModel : Screen
    {
        private String _inputFileName;
        private CSVReader _fileReader;
        private Model _model;
        private Model _primaryModel;
        private ModelDataAccess _modelDAO;
        private ModelDimensionDataAccess _modelDimensionDAO;
        private CsvColumnMappingDataAccess _csvColumnMappingDAO;
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

                CSVFields = new List<string>();
                for(int i = 0; i< _fileReader.Header.Length; i++)
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
            _primaryModel = Models.ElementAt(0);
            List<CsvColumnMapping> _primaryModelColumns = _csvColumnMappingDAO.GetAll(_primaryModel.Id);
            List<CsvColumnMapping> _csvColumns = _csvColumnMappingDAO.GetAll(Context.ModelId);
            CSVFieldMappings = new BindableCollection<CSVFieldMapping>();
            foreach (CsvColumnMapping _primaryModelCsvColumn in _primaryModelColumns)
            {
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
        }
 


        public void ImportData()
        {
            if (String.IsNullOrEmpty(_inputFileName))
            {
                MessageBox.Show("Please select a file!");
                return;
            }
            CSVDataLoader loader = new CSVDataLoader(_fileReader);
            loader.Load();
            _model.HasData = true;
            _modelDAO.Update(_model);
            MessageBox.Show("File imported successfully.");
        }
    }

    public class CSVFieldMapping
    {
        public String PrimayModelColumnName { get; set; }
        public String ColumnName { get; set; }
        public String DefaultValue { get; set; }
        public List<String> CSVFields { get; set; }
    }
}
