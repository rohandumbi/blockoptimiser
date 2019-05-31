using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace blockoptimiser.Views
{
    /// <summary>
    /// Interaction logic for ProcessDefinitionView.xaml
    /// </summary>
    public partial class ProcessEditView : MetroWindow
    {
        private ProcessDataAccess ProcessDAO;
        private FieldDataAccess FieldDAO;
        private ModelDataAccess ModelDAO;
        private List<Field> Fields;
        private List<String> FieldNames;
        private String SelectedFieldName;
        public List<Model> Models;
        private Process UpdatedProcess;
        public ProcessEditView(Process process)
        {
            UpdatedProcess = process;
            InitializeComponent();
            ProcessDAO = new ProcessDataAccess();
            ModelDAO = new ModelDataAccess();
            Models = ModelDAO.GetAll(Context.ProjectId);
            Name.Text = UpdatedProcess.Name;
            BindAllModels();
        }

        private void BindAllModels()
        {
            foreach (Model model in Models)
            {
                foreach (ProcessModelMapping mapping in UpdatedProcess.Mapping)
                {
                    if (model.Id == mapping.ModelId) {
                        model.CheckStatus = true;
                        model.FilterString = mapping.FilterString;
                    }
                }
            }
            ModelMapping.ItemsSource = Models;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            String ProcessName = Name.Text;
            if (ProcessName == null || ProcessName == "")
            {
                MessageBox.Show("Enter manadatory field NAME");
                return;
            }
            List<Model> SelectedModels = new List<Model>();
            //MessageBox.Show(SelectedModels.Count.ToString());
            //Process newProcess = new Process();
            //newProcess.ProjectId = Context.ProjectId;
            //newProcess.Name = ProcessName;
            UpdatedProcess.Mapping = new List<ProcessModelMapping>();
            foreach (Model model in Models)
            {
                if (model.CheckStatus == true)
                {
                    UpdatedProcess.Mapping.Add(new ProcessModelMapping
                    {
                        ModelId = model.Id,
                        FilterString = model.FilterString
                    });
                }
            }
            try
            {
                ProcessDAO.UpdateModelMapping(UpdatedProcess);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this.Close();
        }
    }
}
