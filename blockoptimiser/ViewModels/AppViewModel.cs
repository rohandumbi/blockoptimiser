using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using Caliburn.Micro;
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
        //private List<Department> departments;
        private List<MenuItem> menuItems;
        private List<ProjectDataModel> ProjectDataModels;
        private ProjectDataDataAccess ProjectDataDAO;
        private ProjectDataAccess ProjectDAO;
        private String _newModelName;
        private MenuItem ProjectMenu;
        private MenuItem DataImportMenu;
        private MenuItem GeoTechMenu;
        public AppViewModel()
        {
            ProjectDAO = new ProjectDataAccess();
            ProjectDataDAO = new ProjectDataDataAccess();
            ProjectModel Project = ProjectDAO.Get(Context.ProjectId);
            ProjectMenu = new MenuItem(Project.Name);
            ProjectDataModels = ProjectDataDAO.GetAll(Context.ProjectId);
            DataImportMenu = new MenuItem("Data Import");
            foreach (ProjectDataModel model in ProjectDataModels)
            {
                DataImportMenu.ChildMenuItems.Add(new MenuItem(model.Name));
            }
            GeoTechMenu = new MenuItem("Geotech/Process");
            ProjectMenu.ChildMenuItems.Add(DataImportMenu);
            ProjectMenu.ChildMenuItems.Add(GeoTechMenu);

            MenuItems = new List<MenuItem>()
            {
                ProjectMenu
            };
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
                NotifyOfPropertyChange("Departments");
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
            DataImportMenu.ChildMenuItems.Add(new MenuItem(newModel.Name));
            NotifyOfPropertyChange("MenuItems");
        }
    }
}
