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
        private Scenario Scenario;
        private ModelDataAccess ModelDAO;

        public UnitItem SelectedUnit { get; set; }
        public List<UnitItem> UnitItems { get; set; }

        private ProcessDataAccess ProcessDAO;
        private ProductDataAccess ProductDAO;
        private ProductJoinDataAccess ProductJoinDAO;

        public List<Product> Products;
        public List<String> ProductJoins;
        public List<Process> Processes;
        private List<Model> Models;

        public BindableCollection<ProcessLimit> ProcessLimits { get; set; }


        public ProcessLimitViewModel()
        {
            ProcessLimitDAO = new ProcessLimitDataAccess();
            ProcessLimits = new BindableCollection<ProcessLimit>(ProcessLimitDAO.GetProcessLimits());

            foreach (ProcessLimit processLimitmodel in ProcessLimits)
            {
                processLimitmodel.PropertyChanged += processLimit_PropertyChanged;
                foreach (ProcessLimitYearMapping processLimitYearMapping in processLimitmodel.ProcessLimitYearMapping)
                {
                    processLimitYearMapping.PropertyChanged += processLimitYearMapping_PropertyChanged;
                }
            }

            ScenarioDAO = new ScenarioDataAccess();
            Scenario = ScenarioDAO.Get(Context.ScenarioId);

            ProcessDAO = new ProcessDataAccess();
            ProductDAO = new ProductDataAccess();
            ProductJoinDAO = new ProductJoinDataAccess();
            ModelDAO = new ModelDataAccess();

            Processes = ProcessDAO.GetAll(Context.ProjectId);
            Products = ProductDAO.GetAll(Context.ProjectId);
            ProductJoins = ProductJoinDAO.GetProductJoins(Context.ProjectId);
            Models = ModelDAO.GetAll(Context.ProjectId);


            UnitItems = new List<UnitItem>();

            foreach (Process process in Processes)
            {
                UnitItems.Add(new UnitItem(process.Name, process.Id, ProcessLimit.ITEM_TYPE_PROCESS));
            }

            foreach (Product product in Products)
            {
                UnitItems.Add(new UnitItem(product.Name, product.Id, ProcessLimit.ITEM_TYPE_PRODUCT));
            }

            foreach (String productJoin in ProductJoins)
            {
                UnitItems.Add(new UnitItem(productJoin, 0, ProcessLimit.ITEM_TYPE_PRODUCT_JOIN));
            }

            foreach (Model model in Models)
            {
                UnitItems.Add(new UnitItem(model.Name, model.Id, ProcessLimit.ITEM_TYPE_MODEL));
            }

            this.ProcessLimitColumns = new ObservableCollection<DataGridColumn>();
            this.GenerateDefaultColumns();
        }

        private void processLimit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ProcessLimit UpdatedProcessLimit = (ProcessLimit)sender;
            ProcessLimitDAO.Update(UpdatedProcessLimit);
        }

        private void processLimitYearMapping_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ProcessLimitYearMapping UpdatedProcessLimitYearMappingModel = (ProcessLimitYearMapping)sender;
            ProcessLimit UpdatedProcessLimitModel = GetProcessLimitById(UpdatedProcessLimitYearMappingModel.ProcessLimitId);
            if (UpdatedProcessLimitModel == null)
            {
                MessageBox.Show("Could not find process limit to update. Contact administrator");
                return;
            }
            ProcessLimitDAO.DeleteProcessLimitMapping(UpdatedProcessLimitModel.Id);
            ProcessLimitDAO.InsertProcessLimitMapping(UpdatedProcessLimitModel);
            UpdateCollection();
        }

        public void UpdateCollection()
        {
            ProcessLimits = new BindableCollection<ProcessLimit>(ProcessLimitDAO.GetProcessLimits());
            foreach (ProcessLimit processLimitmodel in ProcessLimits)
            {
                processLimitmodel.PropertyChanged += processLimit_PropertyChanged;
                foreach (ProcessLimitYearMapping processLimitYearMapping in processLimitmodel.ProcessLimitYearMapping)
                {
                    processLimitYearMapping.PropertyChanged += processLimitYearMapping_PropertyChanged;
                }
            }
            NotifyOfPropertyChange(() => ProcessLimits);
        }

        private ProcessLimit GetProcessLimitById(int id)
        {
            ProcessLimit model = null;
            foreach (ProcessLimit processLimit in ProcessLimits)
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
                new DataGridTextColumn { Header = "Item", Binding = new Binding("ItemName"), IsReadOnly = true });
            this.ProcessLimitColumns.Add(
                new DataGridCheckBoxColumn { Header = "Is Used", Binding = new Binding("IsUsed") });

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
            ProcessLimit newProcessLimitModel = new ProcessLimit
            {
                ScenarioId = Context.ScenarioId,
                ItemName = SelectedUnit.Name,
                ItemId = SelectedUnit.UnitId,
                ItemType = SelectedUnit.UnitType,
                IsUsed = true
            };
            List<ProcessLimitYearMapping> NewProcessLimitYearMapping = new List<ProcessLimitYearMapping>();
            for (int i = 0; i < Scenario.TimePeriod; i++)
            {
                ProcessLimitYearMapping TempModel = new ProcessLimitYearMapping();
                int CurrentYear = Scenario.StartYear + i;
                TempModel.Year = CurrentYear;
                TempModel.Value = 0;
                NewProcessLimitYearMapping.Add(TempModel);
            }
            newProcessLimitModel.ProcessLimitYearMapping = NewProcessLimitYearMapping;
            try
            {
                ProcessLimitDAO.InsertProcessLimit(newProcessLimitModel);
                ProcessLimits.Add(newProcessLimitModel);
                UpdateCollection();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
