using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
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
    public partial class ProcessDefinitionView : Window
    {
        private ProcessDataAccess ProcessDAO;
        private FieldDataAccess FieldDAO;
        private ModelDataAccess ModelDAO;
        private List<Field> Fields;
        private List<String> FieldNames;
        private String SelectedFieldName;
        public List<Model> Models;
        public ProcessDefinitionView()
        {
            InitializeComponent();
            ProcessDAO = new ProcessDataAccess();
            ModelDAO = new ModelDataAccess();
            Models = ModelDAO.GetAll(Context.ProjectId);
            BindAllModels();
        }

        private void BindAllModels()
        {
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
            Process newProcess = new Process();
            newProcess.ProjectId = Context.ProjectId;
            newProcess.Name = ProcessName;
            newProcess.Mapping = new List<ProcessModelMapping>();
            foreach (Model model in Models)
            {
                if (model.CheckStatus == true)
                {
                    newProcess.Mapping.Add(new ProcessModelMapping {
                        ModelId = model.Id,
                        FilterString = model.FilterString
                    });
                }
            }
            ProcessDAO.Insert(newProcess);
            this.Close();
        }
    }
}
