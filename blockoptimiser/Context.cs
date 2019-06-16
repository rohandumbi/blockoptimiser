using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace blockoptimiser
{
    class Context
    {
        public static int ProjectId = -1;
        public static int ModelId = -1;
        public static int ScenarioId = -1;

        public static void ExportProject() {
            MessageBox.Show("Exporting project: " + Context.ProjectId);
        }
    }
}
