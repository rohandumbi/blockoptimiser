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

        private static Project exportedProject;
        private static List<Model> exportedModels;
        private static List<Field> exportedFields;
        private static List<ModelDimension> exportedModelDimensions;

        public static void ExportProject() {
            MessageBox.Show("Exporting project: " + Context.ProjectId);
            ExportProjectsTable();
            ExportModelTable();
            ExportModelDimensionTable();
            ExportFieldTable();
        }

        private static void ExportProjectsTable()
        {
            ProjectDataAccess ProjectDAO = new ProjectDataAccess();
            exportedProject = ProjectDAO.Get(Context.ProjectId);
            Console.WriteLine("=============Projects Table=============");
            Console.WriteLine(exportedProject.ToString());
        }

        private static void ExportModelTable()
        {
            ModelDataAccess ModelDAO = new ModelDataAccess();
            exportedModels = ModelDAO.GetAll(Context.ProjectId);
            Console.WriteLine("===============Model Table==============");
            foreach (Model model in exportedModels)
            {
                Console.WriteLine(model.ToString());
            }
        }

        private static void ExportModelDimensionTable()
        {
            exportedModelDimensions = new List<ModelDimension>();
            ModelDimensionDataAccess ModelDimensionDAO = new ModelDimensionDataAccess();
            Console.WriteLine("==========Model Dimension Table========");
            foreach (Model model in exportedModels)
            {
                List<ModelDimension> modelDimensions = ModelDimensionDAO.GetAll(model.Id);
                foreach (ModelDimension modelDimension in modelDimensions)
                {
                    exportedModelDimensions.Add(modelDimension);
                }
            }
            foreach (ModelDimension modelDimension in exportedModelDimensions)
            {
                Console.WriteLine(modelDimension.ToString());
            }
        }

        private static void ExportFieldTable()
        {
            FieldDataAccess FieldDAO = new FieldDataAccess();
            exportedFields = FieldDAO.GetAll(Context.ProjectId);
            Console.WriteLine("===============Field Table==============");
            //TODO: how to handle associated field id
            foreach (Field field in exportedFields)
            {
                Console.WriteLine(field.ToString());
            }
        }


    }
}
