﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class Field
    {
        public const int DATA_TYPE_GROUP_BY = 1;
        public const int DATA_TYPE_ADDITIVE = 2;
        public const int DATA_TYPE_GRADE = 3;

        public int Id { get; set; }
        public int ProjectId { get; set; }
        public String Name { get; set; }
        public int DataType { get; set; }
        public String DataTypeName { get; set; }
        public int AssociatedField { get; set; }
        public String AssociatedFieldName { get; set; }
    }
}
