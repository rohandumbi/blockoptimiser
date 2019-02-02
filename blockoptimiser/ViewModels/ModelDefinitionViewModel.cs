﻿using blockoptimiser.DataAccessClasses;
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
        public List<string> CSVFields { get; set; }
        public BindableCollection<Field> Fields { get; set; }
        public BindableCollection<CsvColumnMapping> CSVFieldMappings { get; set; }
        public BindableCollection<ModelDimension> ModelDimensions { get; set; }

        public String ModelBearing { get; set; }
        public ModelDefinitionViewModel()
        {
            Fields = new BindableCollection<Field>(new FieldDataAccess().GetAll(Context.ProjectId));
            CSVFieldMappings = new BindableCollection<CsvColumnMapping>(new CsvColumnMappingDataAccess().GetAll(1));
            ModelDimensions = new BindableCollection<ModelDimension>(new ModelDimensionDataAccess().GetAll(Context.ModelId));
        }
 
        public String InputFile
        {
            set {
                _inputFileName = value;
                Console.WriteLine("Input file name is " + _inputFileName);
                _fileReader = new CSVReader(_inputFileName);

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
