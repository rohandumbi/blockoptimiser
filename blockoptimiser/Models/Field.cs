using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class Field : INotifyPropertyChanged
    {
        private UnitItem _selectedDataTypeUnitItem;
        private UnitItem _selectedAssocitedFieldUnitItem;
        private String _dataTypeName;
        private int _dataType;

        public const int DATA_TYPE_GROUP_BY = 1;
        public const int DATA_TYPE_ADDITIVE = 2;
        public const int DATA_TYPE_GRADE = 3;

        public int Id { get; set; }
        public int ProjectId { get; set; }
        public String Name { get; set; }
        public Boolean CheckStatus { get; set; }
        public int DataType {
            get {
                return _dataType;
            }
            set {
                _dataType = value;
            }
        }
        public String DataTypeName {
            get {
                switch (DataType)
                {
                    case DATA_TYPE_GROUP_BY:
                        _dataTypeName = "groupby"; break;
                    case DATA_TYPE_ADDITIVE:
                        _dataTypeName = "additive"; break;
                    case DATA_TYPE_GRADE:
                        _dataTypeName = "grade"; break;
                }
                return _dataTypeName;
            }
            set {
                _dataTypeName = value;
            }
        }
        public int AssociatedField { get; set; }
        public String AssociatedFieldName { get; set; }

        /* temp hack of adding additional fields for updating data from table*/
        public List<UnitItem> AssocitedFieldUnitItems { get; set; }
        public UnitItem SelectedAssocitedFieldUnitItem {
            get { return _selectedAssocitedFieldUnitItem; }
            set
            {
                _selectedAssocitedFieldUnitItem = value;
                OnPropertyChanged("SelectedAssocitedFieldUnitItem");
            }
        }
        public List<UnitItem> DataTypeUnitItems { get; set; }
        public UnitItem SelectedDataTypeUnitItem {
            get { return _selectedDataTypeUnitItem; }
            set
            {
                _selectedDataTypeUnitItem = value;
                OnPropertyChanged("SelectedDataTypeUnitItem");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
