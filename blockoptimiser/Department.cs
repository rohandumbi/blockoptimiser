﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser
{
    public class Department : ViewModelBase
    {
        private List<Course> courses;

        public Department(string depname)
        {
            DepartmentName = depname;
            Courses = new List<Course>()
            {
                new Course("Course1"),
                new Course("Course2")
            };
        }

        public List<Course> Courses
        {
            get
            {
                return courses;
            }
            set
            {
                courses = value;
                OnPropertyChanged("Courses");
            }
        }

        public string DepartmentName
        {
            get;
            set;
        }
    }
}
