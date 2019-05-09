using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace blockoptimiser.ViewModels
{
    public class V2PeriodViewModel : Screen
    {
        public List<Scenario> Scenarios { get; set; }
        private ScenarioDataAccess ScenarioDAO;
        private Scenario _selectedScenario;
        private String _scenarioName;
        private int _startYear;
        private int _timePeriod;
        public String DiscountFactor { get; set; }
        public Scenario SelectedItem {
            get { return _selectedScenario; }
            set {
                _selectedScenario = value;
                if (value != null)
                {
                    Context.ScenarioId = _selectedScenario.Id;
                }
            }
        }

        public V2PeriodViewModel()
        {
            ScenarioDAO = new ScenarioDataAccess();
            Scenarios = ScenarioDAO.GetAll();
        }

        public String ScenarioName
        {
            set { _scenarioName = value; }
        }

        public int StartYear
        {
            set { _startYear = value; }
        }

        public int TimePeriod
        {
            set { _timePeriod = value; }
        }

        public void CreateScenario()
        {
            int x = 0;
            bool validDF = Int32.TryParse(DiscountFactor, out x);
            if (!validDF)
            {
                MessageBox.Show("Please select a valid integral value for discount factor");
                return;
            }
            Scenario newScenario = new Scenario
            {
                ProjectId = Context.ProjectId,
                Name = _scenarioName,
                StartYear = _startYear,
                TimePeriod = _timePeriod,
                DiscountFactor = x
            };
            ScenarioDAO.Insert(newScenario);
            //Scenarios.Add(newScenario);
            Scenarios = ScenarioDAO.GetAll();
            NotifyOfPropertyChange("Scenarios");
        }
    }
}
