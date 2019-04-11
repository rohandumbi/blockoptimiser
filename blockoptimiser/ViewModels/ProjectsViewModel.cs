using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace blockoptimiser.ViewModels
{
    class ProjectsViewModel : Screen
    {
        public BindableCollection<Project> Projects { get; set; }
        private ProjectDataAccess _projectDAO;
        private String _projectName;
        private String _projectDescription;

        private readonly IEventAggregator _eventAggregator;

        public ProjectsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _projectDAO = new ProjectDataAccess();
            LoadProjects();
        }

        private void LoadProjects()
        {
            Projects = new BindableCollection<Project>(_projectDAO.GetAll());
        }

        public String ProjectName
        {
            set { _projectName = value; }
        }

        public String ProjectDescription
        {
            set { _projectDescription = value; }
        }

        public Project SelectedItem
        {
            set
            {
                if (value != null)
                {
                    Context.ProjectId = value.Id;
                }
               // _eventAggregator.PublishOnUIThread("loaded:project");
            }
        }

        public void LoadProject()
        {
            if (Context.ProjectId <= 0)
            {
                MessageBox.Show("Please select a project.");
                return;
            }
            _eventAggregator.PublishOnUIThread("loaded:project");
        }

        public void CloneProject()
        {
            if (Context.ProjectId <= 0)
            {
                MessageBox.Show("Please select a project.");
                return;
            }
            _projectDAO.Clone(Context.ProjectId);
            LoadProjects();
            NotifyOfPropertyChange("Projects");
        }

        public void CreateProject()
        {
            Project newProject = new Project
            {
                Name = _projectName,
                Description = _projectDescription
            };
            _projectDAO.Insert(newProject);
            Projects.Add(newProject);
            // Add Required Fields
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

            NotifyOfPropertyChange("Projects");
        }
    }
}
