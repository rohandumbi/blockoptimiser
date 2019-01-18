using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser
{
    public class MenuItem : ViewModelBase
    {
        private List<MenuItem> childMenuItems;

        public MenuItem(string name, string category)
        {
            MenuLabel = name;
            Category = category;
            ChildMenuItems = new List<MenuItem>();
        }

        public List<MenuItem> ChildMenuItems
        {
            get
            {
                return childMenuItems;
            }
            set
            {
                childMenuItems = value;
                OnPropertyChanged("ChildMenuItems");
            }
        }

        public string MenuLabel
        {
            get;
            set;
        }

        public string Category
        {
            get;
            set;
        }
    }
}
