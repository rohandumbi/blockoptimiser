using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace blockoptimiser.ViewModels
{
    public class BenchLimitViewModel : Screen
    {
        private BenchLimitDataAccess BenchLimitDAO;
        private ModelDataAccess ModelDAO;
        public UnitItem SelectedUnit { get; set; }
        public List<UnitItem> UnitItems { get; set; }
        private List<Model> Models;
        public BindableCollection<BenchLimit> BenchLimits { get; set; }

        public BenchLimitViewModel()
        {
            BenchLimitDAO = new BenchLimitDataAccess();
            BenchLimits = new BindableCollection<BenchLimit>(BenchLimitDAO.GetAll());
            foreach (BenchLimit benchLimit in BenchLimits)
            {
                benchLimit.PropertyChanged += benchLimit_PropertyChanged;
            }
            ModelDAO = new ModelDataAccess();
            Models = ModelDAO.GetAll(Context.ProjectId);
            UnitItems = new List<UnitItem>();
            foreach (Model model in Models)
            {
                UnitItems.Add(new UnitItem(model.Name, model.Id, ProcessLimit.ITEM_TYPE_MODEL));
            }
        }

        private void benchLimit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            BenchLimit UpdatedBenchLimit = (BenchLimit)sender;
            BenchLimitDAO.Update(UpdatedBenchLimit);
        }

        public void CreateBenchLimit()
        {
            if (SelectedUnit == null)
            {
                MessageBox.Show("Please select a valid model.");
                return;
            }
            BenchLimit newBenchLimitModel = new BenchLimit
            {
                ScenarioId = Context.ScenarioId,
                ModelName = SelectedUnit.Name,
                ModelId = SelectedUnit.UnitId,
                IsUsed = true,
                Value = 0
            };
            try
            {
                BenchLimitDAO.Insert(newBenchLimitModel);
                BenchLimits.Add(newBenchLimitModel);
                UpdateCollection();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void UpdateCollection()
        {
            BenchLimits = new BindableCollection<BenchLimit>(BenchLimitDAO.GetAll());
            foreach (BenchLimit benchLimit in BenchLimits)
            {
                benchLimit.PropertyChanged += benchLimit_PropertyChanged;
            }
            NotifyOfPropertyChange(() => BenchLimits);
        }
    }
}
