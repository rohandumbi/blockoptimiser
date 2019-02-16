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
  
    public class ProcessLimitViewModel: Screen
    {
        private ProcessLimitDataAccess ProcessLimitDAO;
        private ScenarioDataAccess ScenarioDAO;
        private ScenarioModel Scenario;

        public UnitItem SelectedUnit { get; set; }
        public List<UnitItem> UnitItems { get; set; }

        private ProcessDataAccess ProcessDAO;
        private ProductDataAccess ProductDAO;
        private ProductJoinDataAccess ProductJoinDAO;

        public List<Product> Products;
        public List<String> ProductJoins;
        public List<Process> Processes;

        public BindableCollection<ProcessLimitModel> ProcessLimits { get; set; }


        public ProcessLimitViewModel()
        {
            ProcessLimitDAO = new ProcessLimitDataAccess();
            ProcessLimits = new BindableCollection<ProcessLimitModel>(ProcessLimitDAO.GetProcessLimits());

            foreach (ProcessLimitModel processLimitmodel in ProcessLimits)
            {
                processLimitmodel.PropertyChanged += processLimit_PropertyChanged;
                foreach (ProcessLimitYearMappingModel processLimitYearMapping in processLimitmodel.ProcessLimitYearMapping)
                {
                    processLimitYearMapping.PropertyChanged += processLimitYearMapping_PropertyChanged;
                }
            }

            ScenarioDAO = new ScenarioDataAccess();
            Scenario = ScenarioDAO.GetScenario(Context.ScenarioId);

            ProcessDAO = new ProcessDataAccess();
            ProductDAO = new ProductDataAccess();
            ProductJoinDAO = new ProductJoinDataAccess();

            Processes = ProcessDAO.GetAll(Context.ProjectId);
            Products = ProductDAO.GetAll(Context.ProjectId);
            ProductJoins = ProductJoinDAO.GetProductJoins(Context.ProjectId);


            UnitItems = new List<UnitItem>();

            foreach (Process process in Processes)
            {
                UnitItems.Add(new UnitItem(process.Name, process.Id, ProcessLimitModel.ITEM_TYPE_PROCESS));
            }

            foreach (Product product in Products)
            {
                UnitItems.Add(new UnitItem(product.Name, product.Id, ProcessLimitModel.ITEM_TYPE_PRODUCT));
            }

            foreach (String productJoin in ProductJoins)
            {
                UnitItems.Add(new UnitItem(productJoin, 0, ProcessLimitModel.ITEM_TYPE_PRODUCT_JOIN));
            }

            this.ProcessLimitColumns = new ObservableCollection<DataGridColumn>();
            this.GenerateDefaultColumns();
        }

        private void processLimit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Do stuff with fleet here
            //TruckHubPriorityModel updatedTruckHubPriorityModel = (TruckHubPriorityModel)sender;
            //_truckHubPriorityDataAccess.UpdateTruckHubPriority(updatedTruckHubPriorityModel);
            //NotifyOfPropertyChange(() => TruckHubPriorities);
            MessageBox.Show("detected change");
        }

        private void processLimitYearMapping_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Do stuff with fleet here
            //TruckHubPriorityModel updatedTruckHubPriorityModel = (TruckHubPriorityModel)sender;
            //_truckHubPriorityDataAccess.UpdateTruckHubPriority(updatedTruckHubPriorityModel);
            //NotifyOfPropertyChange(() => TruckHubPriorities);
            ProcessLimitYearMappingModel UpdatedProcessLimitYearMappingModel = (ProcessLimitYearMappingModel)sender;
            ProcessLimitModel UpdatedProcessLimitModel = GetProcessLimitById(UpdatedProcessLimitYearMappingModel.ProcessLimitId);
            if (UpdatedProcessLimitModel == null)
            {
                MessageBox.Show("Could not find process limit to update. Contact administrator");
                return;
            }
            //ProcessLimitDAO.DeleteTruckHourMapping(UpdatedTruckHourModel.Id);
            ProcessLimitDAO.DeleteProcessLimitMapping(UpdatedProcessLimitModel.Id);
            //da.InsertTruckHourMapping(UpdatedTruckHourModel);
            ProcessLimitDAO.InsertProcessLimitMapping(UpdatedProcessLimitModel);
            NotifyOfPropertyChange(() => ProcessLimits);
        }

        private ProcessLimitModel GetProcessLimitById(int id)
        {
            ProcessLimitModel model = null;
            foreach (ProcessLimitModel processLimit in ProcessLimits)
            {
                if (processLimit.Id == id)
                {
                    model = processLimit;
                }
            }
            return model;
        }

        public ObservableCollection<DataGridColumn> ProcessLimitColumns { get; private set; }
        private void GenerateDefaultColumns()
        {
            this.ProcessLimitColumns.Add(
                new DataGridTextColumn { Header = "Item", Binding = new Binding("ItemName") });
            
            for (int i = 0; i < Scenario.TimePeriod; i++)
            {
                int CurrentYear = Scenario.StartYear + i;
                String BindingString = "ProcessLimitYearMapping[" + i + "].Value";
                this.ProcessLimitColumns.Add(new DataGridTextColumn { Header = CurrentYear, Binding = new Binding(BindingString) });
            }
        }

        public void CreateProcessLimit()
        {
            if (SelectedUnit == null)
            {
                MessageBox.Show("Please select a valid Item.");
                return;
            }
            ProcessLimitModel newProcessLimitModel = new ProcessLimitModel
            {
                ScenarioId = Context.ScenarioId,
                ItemName = SelectedUnit.Name,
                ItemId = SelectedUnit.UnitId,
                ItemType = SelectedUnit.UnitType
            };
            List<ProcessLimitYearMappingModel> NewProcessLimitYearMapping = new List<ProcessLimitYearMappingModel>();
            for (int i = 0; i < Scenario.TimePeriod; i++)
            {
                ProcessLimitYearMappingModel TempModel = new ProcessLimitYearMappingModel();
                int CurrentYear = Scenario.StartYear + i;
                TempModel.Year = CurrentYear;
                TempModel.Value = 0;
                NewProcessLimitYearMapping.Add(TempModel);
            }
            newProcessLimitModel.ProcessLimitYearMapping = NewProcessLimitYearMapping;
            ProcessLimitDAO.InsertProcessLimit(newProcessLimitModel);
            ProcessLimits.Add(newProcessLimitModel);
            NotifyOfPropertyChange("ProcessLimits");
        }
    }
}
