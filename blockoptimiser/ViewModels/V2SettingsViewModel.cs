using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using blockoptimiser.Services.LP;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace blockoptimiser.ViewModels
{
    public class V2SettingsViewModel: Conductor<Object>
    {
        private ScenarioDataAccess ScenarioDAO;
        private Scenario _selectedScenario;
        public List<Scenario> AvailableScenarios { get; set; }
        public String DiscountFactor { get; set; }
        public List<String> AvailableYears { get; set; }
        public Scenario SelectedScenario {
            get {
                return _selectedScenario;
            }
            set {
                _selectedScenario = value;
                PopulateYears();
            }
        }
        public String ProgressVisibility { get; set; }
        public String StartYear { get; set; }
        public String EndYear { get; set; }
        public V2SettingsViewModel()
        {
            ScenarioDAO = new ScenarioDataAccess();
            AvailableScenarios = new List<Scenario>();
            AvailableYears = new List<String>();
            AvailableScenarios = ScenarioDAO.GetAll();
            ProgressVisibility = "Hidden";
        }

        private void PopulateYears()
        {
            AvailableYears = new List<String>();
            int StartYear = SelectedScenario.StartYear;
            AvailableYears.Add(StartYear.ToString());
            int PresentYear = StartYear;
            for (int i = 1; i < SelectedScenario.TimePeriod; i++)
            {
                PresentYear++;
                AvailableYears.Add(PresentYear.ToString());
            }
            NotifyOfPropertyChange("AvailableYears");
        }

        private void DisplayIndicator()
        {
            ProgressVisibility = "Visible";
            NotifyOfPropertyChange("ProgressVisibility");
        }
        private void HideIndicator()
        {
            ProgressVisibility = "Hidden";
            NotifyOfPropertyChange("ProgressVisibility");
        }

        public void RunScheduler()
        {
            int StartYearInt = 0;
            int EndYearInt = 0;
            decimal DiscountFactorDecimal = 0;
            if (SelectedScenario == null)
            {
                MessageBox.Show("Please select a scenario");
                return;
            }
            try
            {
                StartYearInt = Int32.Parse(StartYear);
                EndYearInt = Int32.Parse(EndYear);
                DiscountFactorDecimal = Decimal.Parse(DiscountFactor);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return;
            }
            if (!(StartYearInt>0 && EndYearInt>0 && DiscountFactorDecimal>0))
            {
                MessageBox.Show("One of the input values is wrong.");
                return;
            }
            DisplayIndicator();
            RunConfig runconfig = new RunConfig
            {
                ProjectId = Context.ProjectId,
                ScenarioId = SelectedScenario.Id,
                StartYear = StartYearInt,
                EndYear = EndYearInt,
                DiscountFactor = DiscountFactorDecimal
            };
            new CplexSolver().Solve(runconfig);
            HideIndicator();
        }
    }
}
