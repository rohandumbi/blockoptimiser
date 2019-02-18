﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class Product
    {
        public static byte UNIT_TYPE_FIELD = 1;
        public static byte UNIT_TYPE_EXPRESSION = 2;
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public String Name { get; set; }
        public List<int> ProcessIds { get; set; }
        public List<ProductModelMapping> Mapping { get; set; }
        public Boolean CheckStatus { get; set; }
    }

    public class ProductModelMapping
    {
        public int ProductId { get; set; }
        public int ModelId { get; set; }
        public int Unitid { get; set; }
        public int UnitType { get; set; }
    }
}
