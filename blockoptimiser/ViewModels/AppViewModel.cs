using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using blockoptimiser.Services.LP;
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
        private BindableCollection<MenuItem> menuItems { get; set; }
        private List<Model> Models;
        private ModelDataAccess ModelDAO;
        private ModelDimensionDataAccess ModelDimensionDAO;
        private ProjectDataAccess ProjectDAO;
        private String _newModelName;
        private MenuItem ProjectMenu;
        private MenuItem DataImportMenu;
        private MenuItem GeoTechMenu;
        private MenuItem LimitMenu;
        private MenuItem ZoneMenu;
        private String PrimaryModelName;
        public AppViewModel()
        {
            ProjectDAO = new ProjectDataAccess();
            ModelDAO = new ModelDataAccess();
            ModelDimensionDAO = new ModelDimensionDataAccess();
            Project Project = ProjectDAO.Get(Context.ProjectId);
            ProjectMenu = new MenuItem(Project.Name, "project");
            Models = ModelDAO.GetAll(Context.ProjectId);
            DataImportMenu = new MenuItem("Data Import", "data-import");
            foreach (Model model in Models)
            {
                DataImportMenu.ChildMenuItems.Add(new MenuItem(model.Name, "model"));
                if (String.IsNullOrEmpty(PrimaryModelName))
                {
                    PrimaryModelName = model.Name;
                }
            }
            GeoTechMenu = new MenuItem("Geotech/Process", "geotech");
            LimitMenu = new MenuItem("Limits", "limits");
            ZoneMenu = new MenuItem("Zone", "zone");
            LimitMenu.ChildMenuItems.Add(ZoneMenu);
            GeoTechMenu.ChildMenuItems.Add(LimitMenu);
            DataImportMenu.ChildMenuItems.Add(GeoTechMenu);
            ProjectMenu.ChildMenuItems.Add(DataImportMenu);
            //ProjectMenu.ChildMenuItems.Add(GeoTechMenu);

            MenuItems = new BindableCollection<MenuItem>()
            {
                ProjectMenu
            };
            //ActivateItem(new ModelDefinitionViewModel());
        }

        public BindableCollection<MenuItem> MenuItems
        {
            get
            {
                return menuItems;
            }
            set
            {
                menuItems = value;
                NotifyOfPropertyChange(() => MenuItems);
            }
        }

        public void ClickMenu(object e, MouseButtonEventArgs mouseButtonEventArgs)
        {
            // MessageBox.Show(e.Source.ToString());
            MenuItem SelectedMenuItem = (MenuItem)e;
            if (SelectedMenuItem.Category == "geotech")
            {
                ActivateItem(new GeotechContainerViewModel());
            }
            else if (SelectedMenuItem.Category == "model")
            {
                foreach (var Model in Models)
                {
                    if (Model.Name.Equals(SelectedMenuItem.MenuLabel))
                    {
                        Context.ModelId = Model.Id;
                        break;
                    }
                }
                Boolean isPrimaryModel = PrimaryModelName.Equals(SelectedMenuItem.MenuLabel);
                if (isPrimaryModel)
                {
                    ActivateItem(new PrimaryModelDefinitionViewModel());
                }
                else
                {
                    ActivateItem(new ModelDefinitionViewModel());
                }
            }
            else if (SelectedMenuItem.Category == "limits")
            {
                ActivateItem(new LimitsViewModel());
            }
            else if (SelectedMenuItem.Category == "zone")
            {
                ActivateItem(new ZoneViewModel());
            }
            else return;
        }

        public String ModelName
        {
            set { _newModelName = value; }
        }

        public void AddModel()
        {
            Model newModel = new Model
            {
                ProjectId = Context.ProjectId,
                Name = _newModelName
            };
            try
            {
                ModelDAO.Insert(newModel);
            } catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
            String[] Types = { "Origin", "Dimensions", "No Of Blocks" };
            for(int i = 0; i< Types.Length; i++)
            {
                ModelDimension obj = new ModelDimension
                {
                    ModelId = newModel.Id,
                    Type = Types[i]
                };
                ModelDimensionDAO.Insert(obj);
            }

            DataImportMenu.ChildMenuItems.Add(new MenuItem(newModel.Name, "model"));
            //NotifyOfPropertyChange("MenuItems");
            NotifyOfPropertyChange(() => MenuItems);
        }
    }
}
