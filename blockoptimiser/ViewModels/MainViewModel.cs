using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace blockoptimiser.ViewModels
{
    class MainViewModel : Screen
    {
        private string _projectId;
        private String _fleetFileName;
        public String ModelBearing { get; set; }
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

        public String FleetFile
        {
            set { _fleetFileName = value; }
        }

        public void ImportData()
        {
            if (String.IsNullOrEmpty(_fleetFileName))
            {
                MessageBox.Show("Please select a file!");
                return;
            }
            if (String.IsNullOrEmpty(ModelBearing))
            {
                MessageBox.Show("Please provide a value for model bearing!");
                return;
            }
            try
            {
                //IEnumerable<FleetModel> Fleets = ReadCSV(_fleetFileName);
                //_fleetDataAccess.DeleteAll();
                //_fleetDataAccess.InsertFleets(Fleets);
                //LoadFleetList();
                //NotifyOfPropertyChange("Fleets");
                MessageBox.Show("File selected: " + _fleetFileName + " and model bearing: " + ModelBearing);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show("Could not import the file. Please verify the file again.");
                return;
            }
        }
    }
}
