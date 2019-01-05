using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.ViewModels
{
    class MainViewModel : Screen
    {
        private string _projectId;
        public MainViewModel()
        {
            ProjectId = Context.ProjectId.ToString();
        }

        public string ProjectId
        {
            get { return _projectId; }
            set
            {
                _projectId = value;
                NotifyOfPropertyChange(() => ProjectId);
            }
        }
    }
}
