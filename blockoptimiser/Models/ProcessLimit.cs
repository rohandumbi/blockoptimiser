﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Models
{
    public class ProcessLimit : INotifyPropertyChanged
    {
        public static byte ITEM_TYPE_PROCESS = 1;
        public static byte ITEM_TYPE_PRODUCT = 2;
        public static byte ITEM_TYPE_PRODUCT_JOIN = 3;
        public static byte ITEM_TYPE_MODEL = 4;

        private List<ProcessLimitYearMapping> _processLimitYearMapping;
        private Boolean _isUsed;
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }
        public int ScenarioId { get; set; }
        public String ItemName { get; set; }
        public int ItemId { get; set; }
        public byte ItemType { get; set; }

        public Boolean IsUsed
        {
            get { return _isUsed; }
            set
            {
                _isUsed = value;
                OnPropertyChanged("IsUsed");
            }
        }

        public List<ProcessLimitYearMapping> ProcessLimitYearMapping
        {
            get { return _processLimitYearMapping; }
            set
            {
                _processLimitYearMapping = value;
                OnPropertyChanged("ProcessLimitYearMapping");
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

    public class ProcessLimitYearMapping : INotifyPropertyChanged
    {
        private Decimal _value;
        public int ProcessLimitId { get; set; }
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
