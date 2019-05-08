using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.ViewModels
{
    public class V2PeriodViewModel
    {
        public List<Scenario> Scenarios { get; set; }
        private ScenarioDataAccess ScenarioDAO;
        private Scenario _selectedScenario;
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
    }
}
