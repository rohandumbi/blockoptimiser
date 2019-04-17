using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
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
        public List<String> AvailableYears { get; set; }
        public String StartYear { get; set; }
        public String EndYear { get; set; }
        private ScenarioDataAccess ScenarioDAO;

        public SchedulerWindow()
        {
            InitializeComponent();
            ScenarioDAO = new ScenarioDataAccess();
            Scenario loadedScenario = ScenarioDAO.Get(Context.ScenarioId);
            AvailableYears = new List<string>();
            int StartYear = loadedScenario.StartYear;
            AvailableYears.Add(StartYear.ToString());
            int PresentYear = StartYear;
            for (int i=1; i<loadedScenario.TimePeriod; i++)
            {
                PresentYear++;
                AvailableYears.Add(PresentYear.ToString());
            }
            StartYearCombo.ItemsSource = AvailableYears;
            EndYearCombo.ItemsSource = AvailableYears;
        }

        public void button_Click(object sender, RoutedEventArgs e)
        {
            int StartYearInt = 0;
            int EndYearInt = 0;
            Decimal DiscountFactor = 0;
            int Period = 0;
            try
            {
                if (StartYearCombo.SelectedItem == null || EndYearCombo.SelectedItem == null)
                {
                    MessageBox.Show("Select a valid year.");
                    return;
                }
                if (DiscountFactorText.Text == null || DiscountFactorText.Text == "")
                {
                    MessageBox.Show("Provide a valid discount factor.");
                    return;
                }
                if (PeriodText.Text == null || PeriodText.Text == "")
                {
                    MessageBox.Show("Provide a valid period.");
                    return;
                }
                StartYearInt = Int32.Parse((String)StartYearCombo.SelectedItem);
                EndYearInt = Int32.Parse((String)EndYearCombo.SelectedItem);
                DiscountFactor = Decimal.Parse(DiscountFactorText.Text);
                Period = Int32.Parse(PeriodText.Text);
            }
            catch (FormatException)
            {
                //Console.WriteLine($"Unable to parse '{input}'");
                MessageBox.Show("Input valid values for all fields.");
                return;
            }
            if (Context.ScenarioId > 0)
            { 
                RunConfig runconfig = new RunConfig
                {
                    ProjectId = Context.ProjectId,
                    ScenarioId = Context.ScenarioId,
                    StartYear  = StartYearInt,
                    EndYear = EndYearInt,
                    DiscountFactor = DiscountFactor
                };
                new CplexSolver().Solve(runconfig);
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
