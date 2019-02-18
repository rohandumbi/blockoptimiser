using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace blockoptimiser.Services.LP
{
    public class CplexSolver
    {
        private EquationGenerator _generator;

        public CplexSolver()
        {
            _generator = new EquationGenerator();
        }

        public void Solve(int ProjectId, int ScenarioId)
        {
            Scenario scenario = new ScenarioDataAccess().Get(ScenarioId);
            ExecutionContext context = new ExecutionContext(ProjectId, ScenarioId, scenario.DiscountFactor);
            for(int i = 0; i < scenario.TimePeriod; i++)
            {
                int year = scenario.StartYear + i;
                context.Year = year;
                context.Period = (i + 1);
                try
                {
                    _generator.Generate(context);
                } catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                

                // read solution and go for next one. As of now breaking in the first one

                if (i == 0) break;
            }
        }
    }
}
