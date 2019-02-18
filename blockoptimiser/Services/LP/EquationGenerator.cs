using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Services.LP
{
    public class EquationGenerator
    {
        Context ctx;
        public void Generate()
        {
            ctx = new Context();
            //Scenario scenario = new ScenarioDataAccess().Get(Context.ScenarioId);
            FileStream fs = CreateFile();

            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine("maximize");
                WriteObjectiveFunction(sw);
                sw.WriteLine("Subject to");
                WriteConstraints(sw);
                sw.WriteLine("end");
            }
        }

        private FileStream CreateFile()
        {
            Directory.CreateDirectory(@"C:\\blockoptimizor");
            String fileName = @"C:\\blockoptimizor\\blockoptimizor-dump.lp";

            return File.Create(fileName);
        }

        private void WriteObjectiveFunction(StreamWriter sw)
        {
            List<Process> processes = ctx.getProcessList();
            int count = 1;
            foreach(Process process in processes)
            {
                foreach(ProcessModelMapping mapping in process.Mapping)
                {
                    List<Block> blocks = ctx.GetBlocks(mapping.ModelId, mapping.FilterString);

                    foreach(Block block in blocks)
                    {
                        sw.Write(" + B" + block.Id + "p" + count);
                        sw.Write(" + B" + block.Id + "s" + count);
                    }
                }
                count++;
            }
            
        }

        private void WriteConstraints(StreamWriter sw)
        {
            WriteGeotechConstraints(sw);
            WriteProcessLimitConstraints(sw);
            WriteGradeLimitConstraints(sw);
        }

        private void WriteGeotechConstraints(StreamWriter sw)
        {
            sw.WriteLine("\\ Geotech");

        }
        private void WriteProcessLimitConstraints(StreamWriter sw)
        {
            sw.WriteLine("\\Process Limits");
        }

        private void WriteGradeLimitConstraints(StreamWriter sw)
        {
            sw.WriteLine("\\ Grade Limits");
        }

        


    }
}
