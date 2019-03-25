using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class GradeLimit : INotifyPropertyChanged
    {
        public static byte ITEM_TYPE_PRODUCT = 1;
        public static byte ITEM_TYPE_PRODUCT_JOIN = 2;

        private List<GradeLimitYearMapping> _gradeLimitYearMapping;
        private Boolean _isMax;
        private Boolean _isUsed;
        private String _gradeName;
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }
        public int ScenarioId { get; set; }
        public Boolean IsMax {
            get { return _isMax; }
            set
            {
                _isMax = value;
                OnPropertyChanged("IsMax");
            }
        }
        public Boolean IsUsed
        {
            get { return _isUsed; }
            set
            {
                _isUsed = value;
                OnPropertyChanged("IsUsed");
            }
        }
        public String ItemName { get; set; }
        public int ItemId { get; set; }
        public byte ItemType { get; set; }

        public String GradeName {
            get { return _gradeName; }
            set
            {
                _gradeName = value;
                OnPropertyChanged("GradeName");
            }
        }

        public List<GradeLimitYearMapping> GradeLimitYearMapping
        {
            get { return _gradeLimitYearMapping; }
            set
            {
                _gradeLimitYearMapping = value;
                OnPropertyChanged("GradeLimitYearMapping");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class GradeLimitYearMapping : INotifyPropertyChanged
    {
        private Decimal _value;
        public int GradeLimitId { get; set; }
        public int Year { get; set; }
        public Decimal Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
