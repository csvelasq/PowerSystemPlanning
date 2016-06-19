using PowerSystemPlanning;
using PowerSystemPlanning.OPF;
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

namespace PowerSystemPlanningWpfApp.OPF
{
    /// <summary>
    /// Interaction logic for RunEconomicDispatch.xaml
    /// </summary>
    public partial class RunEconomicDispatch : Window
    {
        PowerSystem powerSystem;
        EconomicDispatch ed;

        public RunEconomicDispatch()
        {
            InitializeComponent();
        }

        public RunEconomicDispatch(PowerSystem powSys) : this()
        {
            this.powerSystem = powSys;
        }

        private void btnRunED_Click(object sender, RoutedEventArgs e)
        {
            ed = new EconomicDispatch(this.powerSystem);
            ed.Solve();
            tbTotalCost.Text = ed.TotalGenerationCost.ToString();
            lvPowerGenerated.ItemsSource = ed.PGen_Solution;
        }
    }
}
