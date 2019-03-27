using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

        public void Solve(int ProjectId, int ScenarioId, int StartYear, int EndYear, float DiscountFactor)
        {
            new Thread(() =>
            {
                Console.WriteLine("Scheduler Started");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                Scenario scenario = new ScenarioDataAccess().Get(ScenarioId);
                ExecutionContext context = new ExecutionContext(ProjectId, ScenarioId, scenario.DiscountFactor);
                for (int i = 0; i < scenario.TimePeriod; i++)
                {
                    int year = scenario.StartYear + i;
                    context.Year = year;
                    context.Period = (i + 1);
                    _generator.Generate(context);
                    try
                    {
                        //_generator.Generate(context);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }


                    // read solution and go for next one. As of now breaking in the first one

                    if (i == 0) break;
                }
                stopwatch.Stop();
                // Write hours, minutes and seconds.
                Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);
                Console.WriteLine("Scheduler Ended");
            }).Start();
        }
    }
}
