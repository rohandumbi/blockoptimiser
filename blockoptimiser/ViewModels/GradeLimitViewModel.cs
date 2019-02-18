using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace blockoptimiser.ViewModels
{
    public class GradeLimitViewModel: Screen
    {
        private GradeLimitDataAccess GradeLimitDAO;
        private ScenarioDataAccess ScenarioDAO;
        private Scenario Scenario;

        public UnitItem SelectedUnit { get; set; }
        public List<UnitItem> UnitItems { get; set; }

        private ProcessDataAccess ProcessDAO;
        private ProductDataAccess ProductDAO;
        private ProductJoinDataAccess ProductJoinDAO;

        public List<Product> Products;
        public List<String> ProductJoins;

        public BindableCollection<ProcessLimit> ProcessLimits { get; set; }
        public BindableCollection<GradeLimit> GradeLimits { get; set; }
        public Boolean IsMax { get; set; }


        public GradeLimitViewModel()
        {
            IsMax = false;
            GradeLimitDAO = new GradeLimitDataAccess();
            GradeLimits = new BindableCollection<GradeLimit>(GradeLimitDAO.GetGradeLimits());

            foreach (GradeLimit gradeLimitModel in GradeLimits)
            {
                gradeLimitModel.PropertyChanged += gradeLimit_PropertyChanged;
                foreach (GradeLimitYearMapping gradeLimitYearMapping in gradeLimitModel.GradeLimitYearMapping)
                {
                    gradeLimitYearMapping.PropertyChanged += gradeLimitYearMapping_PropertyChanged;
                }
            }

            ScenarioDAO = new ScenarioDataAccess();
            Scenario = ScenarioDAO.Get(Context.ScenarioId);

            ProcessDAO = new ProcessDataAccess();
            ProductDAO = new ProductDataAccess();
            ProductJoinDAO = new ProductJoinDataAccess();

            Products = ProductDAO.GetAll(Context.ProjectId);
            ProductJoins = ProductJoinDAO.GetProductJoins(Context.ProjectId);


            UnitItems = new List<UnitItem>();

            foreach (Product product in Products)
            {
                UnitItems.Add(new UnitItem(product.Name, product.Id, GradeLimit.ITEM_TYPE_PRODUCT));
            }

            foreach (String productJoin in ProductJoins)
            {
                UnitItems.Add(new UnitItem(productJoin, 0, GradeLimit.ITEM_TYPE_PRODUCT_JOIN));
            }

            this.GradeLimitColumns = new ObservableCollection<DataGridColumn>();
            this.GenerateDefaultColumns();
        }

        private void gradeLimit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Do stuff with fleet here
            //TruckHubPriorityModel updatedTruckHubPriorityModel = (TruckHubPriorityModel)sender;
            //_truckHubPriorityDataAccess.UpdateTruckHubPriority(updatedTruckHubPriorityModel);
            //NotifyOfPropertyChange(() => TruckHubPriorities);
            MessageBox.Show("detected change");
        }

        private void gradeLimitYearMapping_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Do stuff with fleet here
            //TruckHubPriorityModel updatedTruckHubPriorityModel = (TruckHubPriorityModel)sender;
            //_truckHubPriorityDataAccess.UpdateTruckHubPriority(updatedTruckHubPriorityModel);
            //NotifyOfPropertyChange(() => TruckHubPriorities);
            GradeLimitYearMapping UpdatedGradeLimitYearMappingModel = (GradeLimitYearMapping)sender;
            GradeLimit UpdatedGradeLimitModel = GetGradeLimitById(UpdatedGradeLimitYearMappingModel.GradeLimitId);
            if (UpdatedGradeLimitModel == null)
            {
                MessageBox.Show("Could not find grade limit to update. Contact administrator");
                return;
            }
            GradeLimitDAO.DeleteGradeLimitMapping(UpdatedGradeLimitModel.Id);
            GradeLimitDAO.InsertGradeLimitMapping(UpdatedGradeLimitModel);
            UpdateCollection();
        }

        private GradeLimit GetGradeLimitById(int id)
        {
            GradeLimit model = null;
            foreach (GradeLimit gradeLimit in GradeLimits)
            {
                if (gradeLimit.Id == id)
                {
                    model = gradeLimit;
                }
            }
            return model;
        }

        public ObservableCollection<DataGridColumn> GradeLimitColumns { get; private set; }
        private void GenerateDefaultColumns()
        {
            this.GradeLimitColumns.Add(
                new DataGridTextColumn { Header = "Product/Product Join", Binding = new Binding("ItemName") });
            this.GradeLimitColumns.Add(
                new DataGridTextColumn { Header = "Is Max", Binding = new Binding("IsMax") });

            for (int i = 0; i < Scenario.TimePeriod; i++)
            {
                int CurrentYear = Scenario.StartYear + i;
                String BindingString = "GradeLimitYearMapping[" + i + "].Value";
                this.GradeLimitColumns.Add(new DataGridTextColumn { Header = CurrentYear, Binding = new Binding(BindingString) });
            }
        }

        private void UpdateCollection()
        {
            GradeLimits = new BindableCollection<GradeLimit>(GradeLimitDAO.GetGradeLimits());
            NotifyOfPropertyChange("GradeLimits");
        }

        public void CreateProcessLimit()
        {
            if (SelectedUnit == null)
            {
                MessageBox.Show("Please select a valid Item.");
                return;
            }
            //MessageBox.Show("On the right track");
            GradeLimit newGradeLimitModel = new GradeLimit
            {
                ScenarioId = Context.ScenarioId,
                ItemName = SelectedUnit.Name,
                ItemId = SelectedUnit.UnitId,
                ItemType = SelectedUnit.UnitType,
                IsMax = IsMax
            };
            List<GradeLimitYearMapping> NewGradeLimitYearMapping = new List<GradeLimitYearMapping>();
            for (int i = 0; i < Scenario.TimePeriod; i++)
            {
                GradeLimitYearMapping TempModel = new GradeLimitYearMapping();
                int CurrentYear = Scenario.StartYear + i;
                TempModel.Year = CurrentYear;
                TempModel.Value = 0;
                NewGradeLimitYearMapping.Add(TempModel);
            }
            newGradeLimitModel.GradeLimitYearMapping = NewGradeLimitYearMapping;

            GradeLimitDAO.Insert(newGradeLimitModel);
            GradeLimits.Add(newGradeLimitModel);
            UpdateCollection();
        }
    }
}
