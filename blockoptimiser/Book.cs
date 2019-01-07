using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser
{
    public class Book : ViewModelBase
    {
        private string bookname = string.Empty;

        public string BookName
        {
            get
            {
                return bookname;
            }
            set
            {
                bookname = value;
                OnPropertyChanged("BookName");
            }
        }

        public Book(string bookname)
        {
            BookName = bookname;
        }
    }
}
