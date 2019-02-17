using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser
{
    class Context
    {
        public static int ProjectId = -1;
        public static int ModelId = -1;
        public static int ScenarioId = -1;

        private List<Process> processes;
        private List<Product> products;
        private List<String> processJoins;
        private List<String> productJoins;
        public Context()
        {
            if (ProjectId != -1)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            processes = new ProcessDataAccess().GetAll(ProjectId);
            products = new ProductDataAccess().GetAll(ProjectId);
            processJoins = new ProcessJoinDataAccess().GetProcessJoins(ProjectId);
            productJoins = new ProductJoinDataAccess().GetProductJoins(ProjectId);
        }

        public List<Process> getProcessList()
        {
            return processes;
        }

        public List<Block> GetBlocks(int modelId, String condition)
        {
            return new BlockDataAccess().GetBlocks(ProjectId, modelId, condition);
        }
    }
}
