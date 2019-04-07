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
            foreach (Project project in Projects)
            {
                var color = String.Format("#{0:X6}", random.Next(0x1000000));
                project.BackgroundColor = color;
            }
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
    }
}
