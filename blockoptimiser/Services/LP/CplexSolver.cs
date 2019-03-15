﻿using blockoptimiser.DataAccessClasses;
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
        private SchedulerQueueDataAccess _schedulerQueueDataAccess;

        public CplexSolver()
        {
            _generator = new EquationGenerator();
            _schedulerQueueDataAccess = new SchedulerQueueDataAccess();
        }

        public void Solve(int ProjectId, int ScenarioId)
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
                    try
                    {
                        _generator.Generate(context);
                        SchedulerQueue queueItem = new SchedulerQueue
                        {
                            ProjectId = ProjectId,
                            FileName = _generator.FileName
                        };
                        _schedulerQueueDataAccess.Insert(queueItem);
                        Boolean loopcontinue = true;
                        Stopwatch loopstopwatch = new Stopwatch();
                        loopstopwatch.Start();
                        while (loopcontinue && loopstopwatch.ElapsedMilliseconds < 5 * 60 * 1000 ) // If elapsed time is more than 5 mins break
                        {
                            SchedulerQueue uupdateQueueItem = _schedulerQueueDataAccess.Get(queueItem.Id);
                            if(uupdateQueueItem.IsProcessed)
                            {
                                loopcontinue = false;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    
                }
                stopwatch.Stop();
                // Write hours, minutes and seconds.
                Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);
                Console.WriteLine("Scheduler Ended");
            }).Start();
        }
    }
}
