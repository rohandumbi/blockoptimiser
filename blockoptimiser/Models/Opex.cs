using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class Opex
    {
        public static byte MINING_COST = 1;
        public static byte PROCESS_COST = 2;
        public static byte REVENUE = 3;

        public static byte FILTER_TYPE_PROCESS = 1;
        public static byte FILTER_TYPE_PRODUCT = 2;
        public static byte FILTER_TYPE_PROCESS_JOIN = 3;
        public static byte FILTER_TYPE_PRODUCT_JOIN = 4;

        public static byte UNIT_FIELD = 1;
        public static byte UNIT_EXPRESSION = 2;

        public int Id { get; set; }
        public int ScenarioId { get; set; }
        public byte CostType { get; set; }
        public byte FilterType { get; set; }
        public String FilterName { get; set; }
        public Byte UnitType { get; set; }
        public int UnitId { get; set; }
        public Boolean IsUsed { get; set; }
        public List<OpexYearMapping> CostData { get; set; }
    }

    public class OpexYearMapping
    {
        public int OpexId { get; set; }
        public int Year { get; set; }
        public Decimal Value { get; set; }
    }
}
