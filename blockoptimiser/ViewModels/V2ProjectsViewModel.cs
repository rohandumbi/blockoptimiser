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
            NotifyOfPropertyChange("Projects");
        }

        public Project CreateProject(String name, String description)
        {
            Project newProject = new Project
            {
                Name = name,
                Description = description
            };
            try
            {
                ProjectDAO.Insert(newProject);
                LoadProjects();
                //Add required fields
                String[] RequiredFields = { "x", "y", "z", "tonnage" };
                RequiredFieldMappingDataAccess RequiredFieldMappingDAO = new RequiredFieldMappingDataAccess();
                for (int i = 0; i < RequiredFields.Length; i++)
                {
                    RequiredFieldMapping obj = new RequiredFieldMapping
                    {
                        ProjectId = newProject.Id,
                        RequiredFieldName = RequiredFields[i]
                    };
                    RequiredFieldMappingDAO.Insert(obj);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            NotifyOfPropertyChange("Projects");
            return newProject;
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
