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
        public void generate()
        {
            ctx = new Context();
            FileStream fs = CreateFile();

            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine("minimize");
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
            
        }

        private void WriteConstraints(StreamWriter sw)
        {
            
        }
    }
}
