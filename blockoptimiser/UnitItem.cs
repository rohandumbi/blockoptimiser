using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser
{
    public class UnitItem
    {
        public String Name { get; set; }
        public int UnitId { get; set; }
        public byte UnitType { get; set; }
        public UnitItem(String name, int Id, byte type)
        {
            Name = name;
            UnitId = Id;
            UnitType = type;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
