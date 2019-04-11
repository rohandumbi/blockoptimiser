using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class BenchLimit: INotifyPropertyChanged
    {
        private Boolean _isUsed;
        private int _value;
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }
        public int ScenarioId { get; set; }
        public int ModelId { get; set; }
        public int Value {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
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
