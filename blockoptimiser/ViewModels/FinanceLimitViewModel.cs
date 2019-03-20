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
    public class FinanceLimitViewModel: Screen
    {
        private OpexDataAccess OpexDAO;
        private ScenarioDataAccess ScenarioDAO;
        private ProcessDataAccess ProcessDAO;
        private ProductDataAccess ProductDAO;
        private ProductJoinDataAccess ProductJoinDAO;
        private FieldDataAccess FieldDAO;
        private ExpressionDataAccess ExpressionDAO;
        private Scenario Scenario;

        public BindableCollection<Opex> OpexList { get; set; }

        //private List<Opex> OpexList;
        private List<Process> Processes;
        private List<Product> Products;
        private List<String> ProductJoins;
        private List<Field> Fields;
        private List<Models.Expression> Expressions;

        public List<UnitItem> CostTypeUnitItems { get; set; }
        public List<UnitItem> FilterTypeUnitItems { get; set; }
        public List<UnitItem> UnitTypeUnitItems { get; set; }

        private UnitItem _selectedCostTypeUnit;

        public UnitItem SelectedCostTypeUnit {
            get { return _selectedCostTypeUnit; }
            set {
                _selectedCostTypeUnit = value;
                NotifyOfPropertyChange("SelectedCostTypeUnit");
                SelectedUnitTypeUnit = null;
                SelectedFilterTypeUnit = null;
                CostType_SelectionChanged();
                NotifyOfPropertyChange("SelectedUnitTypeUnit");
                NotifyOfPropertyChange("SelectedFilterTypeUnit");
            }
        }
        public UnitItem SelectedFilterTypeUnit { get; set; }
        public UnitItem SelectedUnitTypeUnit { get; set; }

        public Boolean IsUsed { get; set; }

        public Boolean isFilterComboEnabled { get; set; }
        public Boolean isUnitComboEnabled { get; set; }

        public FinanceLimitViewModel()
        {
            ScenarioDAO = new ScenarioDataAccess();
            Scenario = ScenarioDAO.Get(Context.ScenarioId);
            OpexDAO = new OpexDataAccess();
            OpexList = new BindableCollection<Opex>(OpexDAO.GetAll(Context.ScenarioId));

            foreach (Opex opexModel in OpexList)
            {
                opexModel.PropertyChanged += opexModel_PropertyChanged;
                foreach (OpexYearMapping costData in opexModel.CostData)
                {
                    costData.PropertyChanged += costData_PropertyChanged;
                }
            }

            ProductDAO = new ProductDataAccess();
            Products = ProductDAO.GetAll(Context.ProjectId);

            ProcessDAO = new ProcessDataAccess();
            Processes = ProcessDAO.GetAll(Context.ProjectId);

            ProductJoinDAO = new ProductJoinDataAccess();
            ProductJoins = ProductJoinDAO.GetProductJoins(Context.ProjectId);

            FieldDAO = new FieldDataAccess();
            Fields = FieldDAO.GetAll(Context.ProjectId);

            ExpressionDAO = new ExpressionDataAccess();
            Expressions = ExpressionDAO.GetAll(Context.ProjectId);

            PopulateCostTypeUnitItems();
            PopulateFilterTypeUnitItems();
            PopulateUnitTypeUnitItems();

            this.OpexColumns = new ObservableCollection<DataGridColumn>();
            this.GenerateDefaultColumns();
        }

        private void costData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OpexYearMapping UpdatedOpexYearMappingModel = (OpexYearMapping)sender;
            Opex UpdatedOpex = GetOpexById(UpdatedOpexYearMappingModel.OpexId);
            if (UpdatedOpex == null)
            {
                MessageBox.Show("Could not find opex to update. Contact administrator");
                return;
            }
            OpexDAO.DeleteOpexMapping(UpdatedOpex.Id);
            OpexDAO.InsertOpexMapping(UpdatedOpex);
            UpdateCollection();


        }

        private void UpdateCollection()
        {
            OpexList = new BindableCollection<Opex>(OpexDAO.GetAll(Context.ScenarioId));
            foreach (Opex opexModel in OpexList)
            {
                opexModel.PropertyChanged += opexModel_PropertyChanged;
                foreach (OpexYearMapping costData in opexModel.CostData)
                {
                    costData.PropertyChanged += costData_PropertyChanged;
                }
            }
            NotifyOfPropertyChange(() => OpexList);
        }

        private void opexModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Do stuff with fleet here
            Opex UpdatedOpex = (Opex)sender;
            if (e.PropertyName == "FilterName")
            {
                if (!isValidFilter(UpdatedOpex))
                {
                    MessageBox.Show("Process/Product update invalid");
                }
            } else if (e.PropertyName == "UnitName")
            {
                if (!isValidUnit(UpdatedOpex))
                {
                    MessageBox.Show("Expression update invalid");
                }
            }
            OpexDAO.Update(UpdatedOpex);
        }

        private Boolean isValidFilter(Opex UpdatedOpex)
        {
            Boolean isValid = false;
            return isValid;
        }

        private Boolean isValidUnit(Opex UpdatedOpex)
        {
            Boolean isValid = false;
            return isValid;
        }

        private Opex GetOpexById(int id)
        {
            Opex model = null;
            foreach (Opex opex in OpexList)
            {
                if (opex.Id == id)
                {
                    model = opex;
                }
            }
            return model;
        }

        public ObservableCollection<DataGridColumn> OpexColumns { get; private set; }

        private void GenerateDefaultColumns()
        {
            this.OpexColumns.Add(
                new DataGridTextColumn { Header = "Cost", Binding = new Binding("CostName") });
            this.OpexColumns.Add(
                new DataGridTextColumn { Header = "Process/Product", Binding = new Binding("FilterName") });
            this.OpexColumns.Add(
                new DataGridTextColumn { Header = "Expression", Binding = new Binding("UnitName") });

            this.OpexColumns.Add(
                new DataGridCheckBoxColumn { Header = "Is Used", Binding = new Binding("IsUsed") });

            for (int i = 0; i < Scenario.TimePeriod; i++)
            {
                int CurrentYear = Scenario.StartYear + i;
                String BindingString = "CostData[" + i + "].Value";
                this.OpexColumns.Add(new DataGridTextColumn { Header = CurrentYear, Binding = new Binding(BindingString) });
            }
        }

        private void PopulateCostTypeUnitItems()
        {
            CostTypeUnitItems = new List<UnitItem>();
            CostTypeUnitItems.Add(new UnitItem("Mining Cost", 0, Opex.MINING_COST));
            CostTypeUnitItems.Add(new UnitItem("PCost", 0, Opex.PROCESS_COST));
            CostTypeUnitItems.Add(new UnitItem("Rev", 0, Opex.REVENUE));
        }

        private void PopulateFilterTypeUnitItems()
        {
            FilterTypeUnitItems = new List<UnitItem>();
            foreach (Process process in Processes)
            {
                FilterTypeUnitItems.Add(new UnitItem(process.Name, process.Id, Opex.FILTER_TYPE_PROCESS));
            }
            foreach (Product product in Products)
            {
                FilterTypeUnitItems.Add(new UnitItem(product.Name, product.Id, Opex.FILTER_TYPE_PRODUCT));
            }
            foreach (String productJoin in ProductJoins)
            {
                FilterTypeUnitItems.Add(new UnitItem(productJoin, 0, Opex.FILTER_TYPE_PRODUCT_JOIN));
            }
        }

        private void CostType_SelectionChanged() {
            int CostType = SelectedCostTypeUnit.UnitType;
            DisableCombos();
            if (CostType == Opex.MINING_COST)
            {
                isUnitComboEnabled = true;
            }
            else if (CostType == Opex.PROCESS_COST)
            {
                isFilterComboEnabled = true;
            }
            else if (CostType == Opex.REVENUE)
            {
                isFilterComboEnabled = true;
                isUnitComboEnabled = true;
            }
            NotifyOfPropertyChange("isFilterComboEnabled");
            NotifyOfPropertyChange("isUnitComboEnabled");
        }

        private void PopulateUnitTypeUnitItems()
        {
            UnitTypeUnitItems = new List<UnitItem>();
            foreach (Field field in Fields)
            {
                UnitTypeUnitItems.Add(new UnitItem(field.Name, field.Id, Opex.UNIT_FIELD));
            }
            foreach (Models.Expression expression in Expressions)
            {
                UnitTypeUnitItems.Add(new UnitItem(expression.Name, expression.Id, Opex.UNIT_EXPRESSION));
            }
        }

        private void DisableCombos()
        {
            isFilterComboEnabled = false;
            isUnitComboEnabled = false;
            NotifyOfPropertyChange("isFilterComboEnabled");
            NotifyOfPropertyChange("isUnitComboEnabled");
        }

        public void CreateOpex()
        {
            if (SelectedCostTypeUnit == null)
            {
                MessageBox.Show("Please select a valid Cost.");
                return;
            }
            Opex newOpex = new Opex
            {
                ScenarioId = Context.ScenarioId,
                CostType = SelectedCostTypeUnit.UnitType,
                CostName = SelectedCostTypeUnit.Name,
                IsUsed = IsUsed
            };
            if (SelectedFilterTypeUnit != null)
            {
                newOpex.FilterType = SelectedFilterTypeUnit.UnitType;
                newOpex.FilterName = SelectedFilterTypeUnit.Name;
            }
            if (SelectedUnitTypeUnit != null)
            {
                newOpex.UnitId = SelectedUnitTypeUnit.UnitId;
                newOpex.UnitName = SelectedUnitTypeUnit.Name;
                newOpex.UnitType = SelectedUnitTypeUnit.UnitType;
            }
            List<OpexYearMapping> NewOpexYearMapping = new List<OpexYearMapping>();
            for (int i = 0; i < Scenario.TimePeriod; i++)
            {
                OpexYearMapping TempModel = new OpexYearMapping();
                int CurrentYear = Scenario.StartYear + i;
                TempModel.Year = CurrentYear;
                TempModel.Value = 0;
                NewOpexYearMapping.Add(TempModel);
            }
            newOpex.CostData = NewOpexYearMapping;
            try
            {
                OpexDAO.Insert(newOpex);
                OpexList.Add(newOpex);
                UpdateCollection();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
