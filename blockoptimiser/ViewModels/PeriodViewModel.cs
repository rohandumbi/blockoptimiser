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
    public class PeriodViewModel: Screen
    {
        public BindableCollection<ScenarioModel> Scenarios { get; set; }
        private ScenarioDataAccess _scenarioDataAccess;
        private String _scenarioName;
        private int _startYear;
        private int _timePeriod;
        public String DiscountFactor { get; set; }

        private readonly IEventAggregator _eventAggregator;

        private void LoadScenarios()
        {
            Scenarios = new BindableCollection<ScenarioModel>(_scenarioDataAccess.GetScenarios());
        }

        public PeriodViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _scenarioDataAccess = new ScenarioDataAccess();
            LoadScenarios();
        }
        /*public PeriodViewModel()
        {
            //_eventAggregator = eventAggregator;
            _scenarioDataAccess = new ScenarioDataAccess();
            LoadScenarios();
        }*/


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
            bool validDF =Int32.TryParse(DiscountFactor, out x);
            if (!validDF)
            {
                MessageBox.Show("Please select a valid integral value for discount factor");
                return;
            }
            ScenarioModel newScenario = new ScenarioModel
            {
                ProjectId = Context.ProjectId,
                Name = _scenarioName,
                StartYear = _startYear,
                TimePeriod = _timePeriod,
                DiscountFactor = x
            };
            _scenarioDataAccess.InsertScenario(newScenario);
            Scenarios.Add(newScenario);
            NotifyOfPropertyChange("Scenarios");
        }

        public ScenarioModel SelectedItem
        {
            set
            {
                Context.ScenarioId = value.Id;
            }
        }

        public void LoadScenario()
        {
            if (Context.ScenarioId > 0)
            {
                _eventAggregator.PublishOnUIThread("loaded:scenario");
            }
        }
        public void RunScheduler()
        {
            new EquationGenerator().Generate();
        }
    }
}
