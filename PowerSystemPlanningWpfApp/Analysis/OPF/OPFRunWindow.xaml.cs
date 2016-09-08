using PowerSystemPlanning;
using PowerSystemPlanning.Solvers.LDCOPF;
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
    /// Interaction logic for OPFRunWindow.xaml
    /// </summary>
    public partial class OPFRunWindow : Window
    {
        PowerSystem MyPowerSystem { get; set; }
        LoadBlock MyLoadBlock { get; set; }
        OPFModelSolverForLDC OPFSolverForLDC;

        OPFModelResultForLDC _OPFResultsForLDC;
        OPFModelResultForLDC OPFResultsForLDC
        {
            get
            {
                return _OPFResultsForLDC;
            }

            set
            {
                _OPFResultsForLDC = value;
                this.opfResultsControl.OPFResultsForLDC = value;
            }
        }

        public OPFRunWindow()
        {
            InitializeComponent();
        }

        public OPFRunWindow(PowerSystem powerSystem)
            : this()
        {
            MyPowerSystem = powerSystem;
            MyLoadBlock = new LoadBlock(1, 1);
            pnlRun.DataContext = MyLoadBlock;
        }

        private void btnRunOpf_Click(object sender, RoutedEventArgs e)
        {
            //builds the model
            OPFSolverForLDC = new OPFModelSolverForLDC(this.MyPowerSystem, this.MyLoadBlock);
            OPFSolverForLDC.Build();
            //solves the model
            OPFSolverForLDC.Solve();
            //binds results
            OPFResultsForLDC = OPFSolverForLDC.OPFModelResultsForLDC;
        }
    }
}
