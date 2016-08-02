using PowerSystemPlanning;
using PowerSystemPlanning.PlanningModels;
using PowerSystemPlanning.Solvers;
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
        PowerSystemSolverResults OPFSolverResults;
        OPFModelResult OPFResults;

        public RunOPFWindow()
        {
            InitializeComponent();
        }

        public RunOPFWindow(PowerSystem powerSystem) : this()
        {
            this.powerSystem = powerSystem;
            opf = new OPFModel(new PowerSystemDecorator(this.powerSystem));
            opf.Solve();
            this.OPFSolverResults = opf.getResults();
            this.OPFResults = (this.OPFSolverResults.Result as OPFModelResult);
            this.DataContext = this.OPFResults;
            //tbTotalCost.Text = this.OPFResults.TotalGenerationCost.ToString();
            //dgNodalResults.DataContext = OPFResults.NodeOPFResults;
            //dgGeneratorResults.DataContext = OPFResults.GeneratingUnitOPFResults;
            //dgTransmissionLineResults.DataContext = OPFResults.TransmissionLineOPFResults;
        }
    }
}
