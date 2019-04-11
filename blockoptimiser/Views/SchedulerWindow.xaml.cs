using blockoptimiser.Services.LP;
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
    /// Interaction logic for SchedulerWindow.xaml
    /// </summary>
    public partial class SchedulerWindow : Window
    {
        public SchedulerWindow()
        {
            InitializeComponent();
        }

        public void button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Run sliding window.");
            int StartYear = 0;
            int EndYear = 0;
            float DiscountFactor = 0;
            try
            {
                StartYear = Int32.Parse(StartYearText.Text);
                EndYear = Int32.Parse(EndYearText.Text);
                DiscountFactor = float.Parse(DiscountFactorText.Text);
            }
            catch (FormatException)
            {
                //Console.WriteLine($"Unable to parse '{input}'");
                MessageBox.Show("Input valid values for all fields.");
                return;
            }
            if (Context.ScenarioId > 0)
            {
                new CplexSolver().Solve(Context.ProjectId, Context.ScenarioId, StartYear, EndYear, DiscountFactor);
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a scenario.");
                return;
            }
        }
    }
}
