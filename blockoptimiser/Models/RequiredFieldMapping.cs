using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class RequiredFieldMapping : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public String RequiredFieldName { get; set; }
        private String _mappedColumnName;

        public String MappedColumnName
        {
            get { return _mappedColumnName; }
            set
            {
                _mappedColumnName = value;
                OnPropertyChanged("ColumnMapping");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public List<String> mappingOptions { get; set; }

        public override string ToString()
        {
            return Id + "," + ProjectId + "," + RequiredFieldName + "," + MappedColumnName;
        }
    }
}
