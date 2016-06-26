using PowerSystemPlanning;
using PowerSystemPlanning.Solvers.OPF;
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
    /// Interaction logic for RunOPFWindow.xaml
    /// </summary>
    public partial class RunOPFWindow : Window
    {
        PowerSystem powerSystem;
        OPFModel opf;

        public RunOPFWindow()
        {
            InitializeComponent();
        }

        public RunOPFWindow(PowerSystem powerSystem) : this()
        {
            this.powerSystem = powerSystem;
        }

        private void btnRunOPF_Click(object sender, RoutedEventArgs e)
        {
            opf = new OPFModel(this.powerSystem);
            opf.Solve();
            tbTotalCost.Text = opf.TotalGenerationCost.ToString();
            lvPowerGenerated.ItemsSource = opf.PGen_Solution;
        }
    }
}
