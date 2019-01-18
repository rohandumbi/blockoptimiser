﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.ViewModels
{
    public class PrimaryModelDefinitionViewModel: Screen
    {
        public BindableCollection<string> FixedFields { get; set;}
        public List<string> CSVFields { get; set; }
        public List<string> DataTypes { get; set; }
        public BindableCollection<FieldMappingModel> FieldMappings { get; set; }
        public BindableCollection<FixedFieldMappingModel> FixedFieldMappings { get; set; }
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
    }
}
