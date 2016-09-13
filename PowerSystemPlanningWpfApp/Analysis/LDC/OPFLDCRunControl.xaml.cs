using PowerSystemPlanning.PlanningModels;
using PowerSystemPlanning.Solvers.LDCOPF;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PowerSystemPlanning;

namespace PowerSystemPlanningWpfApp.Analysis.LDC
{
    /// <summary>
    /// Interaction logic for OPFLDCRunControl.xaml
    /// </summary>
    public partial class OPFLDCRunControl : UserControl
    {
        public LDCPowerSystemPlanningModel MyLDCPowerSystemPlanningModel { get; set; }
        PowerSystem MyPowerSystem
        {
            get
            {
                return MyLDCPowerSystemPlanningModel.MyPowerSystem;
            }
        }
        LoadDurationCurveByBlocks MyLoadDurationCurve
        {
            get
            {
                return MyLDCPowerSystemPlanningModel.MyLoadDurationCurve;
            }
        }

        LDCOPFModelSolver MyLDCOPFModelSolver;

        public OPFLDCRunControl()
        {
            InitializeComponent();
        }

        private void btnRunLdcOpf_Click(object sender, RoutedEventArgs e)
        {
            //builds the model
            MyLDCOPFModelSolver = new LDCOPFModelSolver(MyPowerSystem, MyLoadDurationCurve);
            MyLDCOPFModelSolver.Build();
            //solves the model
            MyLDCOPFModelSolver.Solve();
            //binds results
            myOpfLdcResultsControl.MyLDCOPFModelResults = MyLDCOPFModelSolver.LDCOPFResults;
        }
    }
}
