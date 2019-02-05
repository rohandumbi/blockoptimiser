using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace blockoptimiser.ViewModels
{
    class ProcessViewModel: Screen
    {
        public void AddProcess()
        {
            MessageBox.Show("add a process.");
        }

        public void AddProduct()
        {
            MessageBox.Show("add a product.");
        }

        public void AddProductJoin()
        {
            MessageBox.Show("add a product join.");
        }
    }
}
