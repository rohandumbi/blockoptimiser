﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.ViewModels
{
    public class AppViewModel : Screen
    {
        private List<Department> departments;
        public AppViewModel()
        {
            Departments = new List<Department>()
            {
                new Department("Department1"),
                new Department("Department2")
            };
        }

        public List<Department> Departments
        {
            get
            {
                return departments;
            }
            set
            {
                departments = value;
                NotifyOfPropertyChange("Departments");
            }
        }
    }
}
