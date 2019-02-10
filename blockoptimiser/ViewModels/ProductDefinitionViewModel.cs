using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace blockoptimiser.ViewModels
{
    public class ProductDefinitionViewModel: Screen
    {
        private ProcessDataAccess ProcessDAO;
        private ProductDataAccess ProductDAO;
        public List<Process> Processes { get; set; }
        public BindableCollection<String> ProcessNames { get; set; }
        public BindableCollection<String> SelectetdProcessNames { get; set; }
        public String Name { get; set; }
        public String SelectedProcessName { get; set; }

        public ProductDefinitionViewModel()
        {
            ProcessDAO = new ProcessDataAccess();
            ProductDAO = new ProductDataAccess();
            Processes = ProcessDAO.GetAll(Context.ProjectId);
            ProcessNames = new BindableCollection<string>();
            foreach (Process process in Processes)
            {
                ProcessNames.Add(process.Name);
            }
        }

        public void AddProduct()
        {
            //MessageBox.Show("Name: " + Name + "  Process:" + SelectedProcessName);
            if (Name == null)
            {
                MessageBox.Show("Mandatory field NAME missing.");
                return;
            }
            if (SelectedProcessName == null)
            {
                MessageBox.Show("Mandatory field PROCESS missing.");
                return;
            }
            Product newProduct = new Product();
            newProduct.ProjectId = Context.ProjectId;
            newProduct.AssociatedProcessId = GetProcessByName(SelectedProcessName).Id;
            newProduct.Name = Name;
            ProductDAO.Insert(newProduct);
        }

        private Process GetProcessByName(String processname)
        {
            Process returnedProcess = Processes.First();
            foreach (Process process in Processes)
            {
                if (process.Name == processname)
                {
                    returnedProcess = process;
                }
            }
            return returnedProcess;
        }
    }
}
