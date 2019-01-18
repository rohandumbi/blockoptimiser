using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace blockoptimiser.ViewModels
{
    public class AppViewModel : Conductor<Object>
    {
        //private List<Department> departments;
        private List<MenuItem> menuItems;
        private List<ProjectDataModel> ProjectDataModels;
        private ProjectDataDataAccess ProjectDataDAO;
        private ProjectDataAccess ProjectDAO;
        private String _newModelName;
        private MenuItem ProjectMenu;
        private MenuItem DataImportMenu;
        private MenuItem GeoTechMenu;
        private Boolean isPrimaryModel = true;
        public AppViewModel()
        {
            ProjectDAO = new ProjectDataAccess();
            ProjectDataDAO = new ProjectDataDataAccess();
            ProjectModel Project = ProjectDAO.Get(Context.ProjectId);
            ProjectMenu = new MenuItem(Project.Name, "project");
            ProjectDataModels = ProjectDataDAO.GetAll(Context.ProjectId);
            DataImportMenu = new MenuItem("Data Import", "data-import");
            foreach (ProjectDataModel model in ProjectDataModels)
            {
                DataImportMenu.ChildMenuItems.Add(new MenuItem(model.Name, "model"));
            }
            GeoTechMenu = new MenuItem("Geotech/Process", "geotech");
            ProjectMenu.ChildMenuItems.Add(DataImportMenu);
            ProjectMenu.ChildMenuItems.Add(GeoTechMenu);

            MenuItems = new List<MenuItem>()
            {
                ProjectMenu
            };
            //ActivateItem(new ModelDefinitionViewModel());
        }

        public List<MenuItem> MenuItems
        {
            get
            {
                return menuItems;
            }
            set
            {
                menuItems = value;
                NotifyOfPropertyChange("MenuItems");
            }
        }

        public void ClickMenu(object e, MouseButtonEventArgs mouseButtonEventArgs)
        {
            // MessageBox.Show(e.Source.ToString());
            if (isPrimaryModel)
            {
                ActivateItem(new PrimaryModelDefinitionViewModel());
            }
            else
            {
                ActivateItem(new ModelDefinitionViewModel());
            }
        }

        public String ModelName
        {
            set { _newModelName = value; }
        }

        public void AddModel()
        {
            ProjectDataModel newModel = new ProjectDataModel
            {
                ProjectId = Context.ProjectId,
                Name = _newModelName,
                Bearing = 1
            };
            ProjectDataDAO.Insert(newModel);
            DataImportMenu.ChildMenuItems.Add(new MenuItem(newModel.Name, "model"));
            NotifyOfPropertyChange("MenuItems");
        }
    }
}
