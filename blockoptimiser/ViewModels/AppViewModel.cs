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
        public AppViewModel()
        {
            ProjectDAO = new ProjectDataAccess();
            ProjectDataDAO = new ProjectDataDataAccess();
            ProjectModel Project = ProjectDAO.Get(Context.ProjectId);
            MenuItem ProjectMenu = new MenuItem(Project.Name);
            ProjectDataModels = ProjectDataDAO.GetAll(Context.ProjectId);
            MenuItem DataImport = new MenuItem("Data Import");
            foreach (ProjectDataModel model in ProjectDataModels)
            {
                DataImport.ChildMenuItems.Add(new MenuItem(model.Name));
            }
            MenuItem GeoTech = new MenuItem("Geotech/Process");
            ProjectMenu.ChildMenuItems.Add(DataImport);
            ProjectMenu.ChildMenuItems.Add(GeoTech);

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
    }
}
