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
    public delegate void TaskCompletedCallBack();
    public class CplexSolver
    {
        private EquationGenerator _generator;
        private SchedulerQueueDataAccess _schedulerQueueDataAccess;

        public CplexSolver()
        {
            _generator = new EquationGenerator();
            _schedulerQueueDataAccess = new SchedulerQueueDataAccess();
        }

        public void Solve(RunConfig runconfig)
        {
            new Thread(() =>
            {
                Console.WriteLine("Scheduler Started");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                Scenario scenario = new ScenarioDataAccess().Get(runconfig.ScenarioId);

                ExecutionContext context = new ExecutionContext(runconfig);
                for (int i = 0; i < (runconfig.EndYear - runconfig.StartYear + 1); i++)
                {
                    int year = runconfig.StartYear + i;
                    context.Year = year;
                    context.Period = (i + 1);
                    try
                    {
                        if (i > 0)
                        {
                            context.LoadMinedBlockList();
                        }
                        _generator.Generate(context);
                        SchedulerQueue queueItem = new SchedulerQueue
                        {
                            ProjectId = runconfig.ProjectId,
                            FileName = _generator.FileName,
                            Year = year
                        };
                        _schedulerQueueDataAccess.Insert(queueItem);
                        Boolean solved = false;
                        Boolean loopcontinue = true;
                        Stopwatch loopstopwatch = new Stopwatch();
                        loopstopwatch.Start();
                        while (loopcontinue && loopstopwatch.ElapsedMilliseconds < 5 * 60 * 1000) // If elapsed time is more than 5 mins break
                        {
                            SchedulerQueue updateQueueItem = _schedulerQueueDataAccess.Get(queueItem.Id);
                            if (updateQueueItem.IsProcessed)
                            {
                                loopcontinue = false;
                                solved = true;
                            }
                            else
                            {
                                Console.WriteLine("Waiting for queue item to be processed. ");
                                Thread.Sleep(5000);
                            }
                        }
                        if (!solved)
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        break;
                    }

                }
                stopwatch.Stop();
                // Write hours, minutes and seconds.
                Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);
                Console.WriteLine("Scheduler Ended");
            }).Start();
        }

        public void Solve(RunConfig runconfig, TaskCompletedCallBack Callback)
        {
            new Thread(() =>
            {
                Console.WriteLine("Scheduler Started");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                Scenario scenario = new ScenarioDataAccess().Get(runconfig.ScenarioId);

                ExecutionContext context = new ExecutionContext(runconfig);
                for (int i = 0; i < (runconfig.EndYear - runconfig.StartYear + 1); i++)
                {
                    int year = runconfig.StartYear + i;
                    context.Year = year;
                    context.Period = (i + 1);
                    try
                    {
                        if(i>0)
                        {
                            context.LoadMinedBlockList();
                        }
                        _generator.Generate(context);
                        SchedulerQueue queueItem = new SchedulerQueue
                        {
                            ProjectId = runconfig.ProjectId,
                            FileName = _generator.FileName,
                            Year = year
                        };
                        _schedulerQueueDataAccess.Insert(queueItem);
                        Boolean solved = false;
                        Boolean loopcontinue = true;
                        Stopwatch loopstopwatch = new Stopwatch();
                        loopstopwatch.Start();
                        while (loopcontinue && loopstopwatch.ElapsedMilliseconds < 5 * 60 * 1000 ) // If elapsed time is more than 5 mins break
                        {
                            SchedulerQueue updateQueueItem = _schedulerQueueDataAccess.Get(queueItem.Id);
                            if(updateQueueItem.IsProcessed)
                            {
                                loopcontinue = false;
                                solved = true;
                            } else
                            {
                                Console.WriteLine("Waiting for queue item to be processed. ");
                                Thread.Sleep(5000);
                            }
                        }
                        if(!solved)
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        Callback();
                        break;
                    }
                    
                }
                stopwatch.Stop();
                // Write hours, minutes and seconds.
                Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);
                Console.WriteLine("Scheduler Ended");
                Callback();
            }).Start();
        }
    }
}
