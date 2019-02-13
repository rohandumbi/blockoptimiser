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

        public Context()
        {
            if (ProjectId != -1)
            {
                LoadData();
            }
        }

        private void LoadData()
        {

        }
    }
}
