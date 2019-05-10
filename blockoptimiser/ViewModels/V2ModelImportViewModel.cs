using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using blockoptimiser.Services.LP;
using blockoptimiser.Views;
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
    public class V2ModelImportViewModel : Conductor<Object>
    {
        private BindableCollection<MenuItem> menuItems { get; set; }
        public List<Model> Models { get; set; }
        private ModelDataAccess ModelDAO;
        private ModelDimensionDataAccess ModelDimensionDAO;
        private ProjectDataAccess ProjectDAO;
        private String _newModelName;
        private String PrimaryModelName;
        public UserControl ModelDefinitionUserControl { get; set; }
        public V2ModelImportViewModel()
        {
            ProjectDAO = new ProjectDataAccess();
            ModelDAO = new ModelDataAccess();
            ModelDimensionDAO = new ModelDimensionDataAccess();
            Project Project = ProjectDAO.Get(Context.ProjectId);
            Models = ModelDAO.GetAll(Context.ProjectId);
            foreach (Model model in Models)
            {
                if (String.IsNullOrEmpty(PrimaryModelName))
                {
                    PrimaryModelName = model.Name;
                }
            }
            ModelDefinitionUserControl = new UserControl();
        }

        public void ClickMenu(object e, MouseButtonEventArgs mouseButtonEventArgs)
        {
            ListBox ModelList = (ListBox)e;
            String SelectedModelName = ((Model)ModelList.SelectedItem).Name;
            foreach (var Model in Models)
            {
                if (Model.Name.Equals(SelectedModelName))
                {
                    Context.ModelId = Model.Id;
                    break;
                }
            }
            Boolean isPrimaryModel = PrimaryModelName.Equals(SelectedModelName);
            if (isPrimaryModel)
            {
                //ActivateItem(new PrimaryModelDefinitionViewModel());
                ModelDefinitionUserControl = new V2PrimaryModelDefinitionView();
            }
            else
            {
                //ActivateItem(new ModelDefinitionViewModel());
                ModelDefinitionUserControl = new V2ModelDefinitionView();
            }
            NotifyOfPropertyChange("ModelDefinitionUserControl");
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
                if (String.IsNullOrEmpty(PrimaryModelName))
                {
                    PrimaryModelName = _newModelName;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            String[] Types = { "Origin", "Dimensions", "No Of Blocks" };
            for (int i = 0; i < Types.Length; i++)
            {
                ModelDimension obj = new ModelDimension
                {
                    ModelId = newModel.Id,
                    Type = Types[i]
                };
                ModelDimensionDAO.Insert(obj);
            }
            Models = ModelDAO.GetAll(Context.ProjectId);
            NotifyOfPropertyChange("Models");
        }
    }
}
