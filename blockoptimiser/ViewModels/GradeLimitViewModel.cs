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
        private FieldDataAccess FieldDAO;
        private Scenario Scenario;

        public UnitItem SelectedUnit { get; set; }
        public List<UnitItem> UnitItems { get; set; }

        private ProcessDataAccess ProcessDAO;
        private ProductDataAccess ProductDAO;
        private ProductJoinDataAccess ProductJoinDAO;

        public List<Product> Products;
        public List<String> ProductJoins;
        private List<Field> Fields;


        public BindableCollection<ProcessLimit> ProcessLimits { get; set; }
        public BindableCollection<GradeLimit> GradeLimits { get; set; }
        public Boolean IsMax { get; set; }

        public String SelectedGradeName { get; set; }


        public GradeLimitViewModel()
        {
            IsMax = false;
            GradeLimitDAO = new GradeLimitDataAccess();
            GradeLimits = new BindableCollection<GradeLimit>(GradeLimitDAO.GetAll(Context.ScenarioId));

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

            FieldDAO = new FieldDataAccess();
            Fields = FieldDAO.GetAll(Context.ProjectId);

            this.GradeLimitColumns = new ObservableCollection<DataGridColumn>();
            this.GenerateDefaultColumns();
        }

        private void gradeLimit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            GradeLimit UpdatedGradeLimit = (GradeLimit)sender;
            if (e.PropertyName == "GradeName")
            {
                if (!isValidGradeUpdate(UpdatedGradeLimit))
                {
                    MessageBox.Show("Invalid grade selected");
                    return;
                }
            }
            GradeLimitDAO.Update(UpdatedGradeLimit);
        }

        private Boolean isValidGradeUpdate(GradeLimit UpdatedGradeLimit)
        {
            Boolean isValidUpdate = false;
            if (UpdatedGradeLimit.ItemType == GradeLimit.ITEM_TYPE_PRODUCT)
            {
                Product product = GetProductById(UpdatedGradeLimit.ItemId);
                List<String> AssociatedGrades = GetAssociatedGrades(product);
                foreach (String AssociatedGradeName in AssociatedGrades)
                {
                    if (AssociatedGradeName == UpdatedGradeLimit.GradeName)
                    {
                        isValidUpdate = true;
                    }
                }
                
            } else if (UpdatedGradeLimit.ItemType == GradeLimit.ITEM_TYPE_PRODUCT_JOIN)
            {
                List<String> AssociatedGradeAliases = ProductJoinDAO.GetGradeAliasNames(UpdatedGradeLimit.ItemName, Context.ProjectId);
                foreach (String AliasName in AssociatedGradeAliases)
                {
                    if (AliasName == UpdatedGradeLimit.GradeName)
                    {
                        isValidUpdate = true;
                    }
                }
            }
            return isValidUpdate;
        }

        private List<String> GetAssociatedGrades(Product selectedProduct)
        {
            List<String> associatedGradeNames = new List<string>();
            foreach (Field field in Fields)
            {
                if (field.DataType == Field.DATA_TYPE_GRADE && field.AssociatedField == selectedProduct.UnitId)
                {
                    associatedGradeNames.Add(field.Name);
                }
            }
            return associatedGradeNames;
        }

        private Product GetProductById(int id)
        {
            Product returnedProduct = null;
            foreach (Product product in Products)
            {
                if (product.Id == id)
                {
                    returnedProduct = product;
                    break;
                }
            }
            return returnedProduct;
        }

        private void gradeLimitYearMapping_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
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
                new DataGridTextColumn { Header = "Product/Product Join", Binding = new Binding("ItemName"), IsReadOnly = true });
            this.GradeLimitColumns.Add(
              new DataGridTextColumn { Header = "Grade", Binding = new Binding("GradeName") });
            //this.GradeLimitColumns.Add(new DataGridComboBoxColumn { Header = "Grade", ItemsSource = "" });
            this.GradeLimitColumns.Add(
                new DataGridCheckBoxColumn { Header = "Is Max", Binding = new Binding("IsMax") });
            this.GradeLimitColumns.Add(
                new DataGridCheckBoxColumn { Header = "Is Used", Binding = new Binding("IsUsed") });

            for (int i = 0; i < Scenario.TimePeriod; i++)
            {
                int CurrentYear = Scenario.StartYear + i;
                String BindingString = "GradeLimitYearMapping[" + i + "].Value";
                this.GradeLimitColumns.Add(new DataGridTextColumn { Header = CurrentYear, Binding = new Binding(BindingString) });
            }
        }

        private void UpdateCollection()
        {
            GradeLimits = new BindableCollection<GradeLimit>(GradeLimitDAO.GetAll(Context.ScenarioId));
            foreach (GradeLimit gradeLimitModel in GradeLimits)
            {
                gradeLimitModel.PropertyChanged += gradeLimit_PropertyChanged;
                foreach (GradeLimitYearMapping gradeLimitYearMapping in gradeLimitModel.GradeLimitYearMapping)
                {
                    gradeLimitYearMapping.PropertyChanged += gradeLimitYearMapping_PropertyChanged;
                }
            }
            NotifyOfPropertyChange("GradeLimits");
        }

        public void CreateGradeLimit()
        {
            if (SelectedUnit == null)
            {
                MessageBox.Show("Please select a valid Item.");
                return;
            }
            if (SelectedGradeName == null || SelectedGradeName == "")
            {
                MessageBox.Show("Please select a valid Grade.");
                return;
            }
            //MessageBox.Show("On the right track");
            GradeLimit newGradeLimitModel = new GradeLimit
            {
                ScenarioId = Context.ScenarioId,
                ItemName = SelectedUnit.Name,
                ItemId = SelectedUnit.UnitId,
                ItemType = SelectedUnit.UnitType,
                IsMax = IsMax,
                GradeName = SelectedGradeName,
                IsUsed = true
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
            try
            {
                GradeLimitDAO.Insert(newGradeLimitModel);
                GradeLimits.Add(newGradeLimitModel);
                UpdateCollection();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
