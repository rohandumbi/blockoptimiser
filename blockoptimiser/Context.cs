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
            ExportProjectsTable();
            ExportModelTable();
        }

        private static void ExportProjectsTable()
        {
            ProjectDataAccess ProjectDAO = new ProjectDataAccess();
            Project exportedProject = ProjectDAO.Get(Context.ProjectId);
            Console.WriteLine("=============Projects Table=============");
            Console.WriteLine(exportedProject.ToString());
        }

        private static void ExportModelTable()
        {
            ModelDataAccess ModelDAO = new ModelDataAccess();
            List<Model> exportedModels = ModelDAO.GetAll(Context.ProjectId);
            Console.WriteLine("===============Model Table==============");
            foreach (Model model in exportedModels)
            {
                Console.WriteLine(model.ToString());
            }
        }
    }
}
