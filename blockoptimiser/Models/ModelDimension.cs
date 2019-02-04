using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class ModelDimension : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int ModelId { get; set; }
        public String Type { get; set; }
        private Decimal _xdim;
        private Decimal _ydim;
        private Decimal _zdim;

        public Decimal XDim {
            get { return _xdim;  }
            set {
                _xdim = value;
                OnPropertyChanged("ModelDimension");
            }
        }
        public Decimal YDim
        {
            get { return _ydim; }
            set
            {
                _ydim = value;
                OnPropertyChanged("ModelDimension");
            }
        }
        public Decimal ZDim {
            get { return _zdim; }
            set
            {
                _zdim = value;
                OnPropertyChanged("ModelDimension");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
