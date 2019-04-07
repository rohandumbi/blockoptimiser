using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace blockoptimiser.ViewModels
{
    class V2ProjectsViewModel : Screen
    {
        public BindableCollection<Project> Projects { get; set; }
        private ProjectDataAccess ProjectDAO;
        private readonly IEventAggregator _eventAggregator;

        public V2ProjectsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            ProjectDAO = new ProjectDataAccess();
            LoadProjects();
        }

        private void LoadProjects()
        {
            Projects = new BindableCollection<Project>(ProjectDAO.GetAll());
            var random = new Random();
            int j = 0;
            for (var i=0; i<Projects.Count; i++)
            {
                j++;
                if (j > 6)
                {
                    j = 1;
                }
                var color = GetBackgroundColor(j);
                Projects[i].BackgroundColor = color;
            }
            //foreach (Project project in Projects)
            //{
            //    var color = String.Format("#{0:X6}", random.Next(0x1000000));
            //    project.BackgroundColor = color;
            //}
        }

        public void ShowProject(object e, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Project selectedProject = e as Project;
            _eventAggregator.PublishOnUIThread("load:projectDetailsFlyout");
        }

        public void AddProject()
        {
            _eventAggregator.PublishOnUIThread("load:addProjectFlyout");
        }

        private String GetBackgroundColor(int index)
        {
            if (index == 1)
            {
                return "Teal";
            }
            else if (index == 2)
            {
                return "DimGray";
            }
            else if (index == 3)
            {
                return "#D2691E";
            }
            else if (index == 4)
            {
                return "#FF842D";
            }
            else if (index == 5)
            {
                return "#1E90FF";
            }
            else if (index == 6)
            {
                return "Green";
            }
            else return "";
        }
    }
}
