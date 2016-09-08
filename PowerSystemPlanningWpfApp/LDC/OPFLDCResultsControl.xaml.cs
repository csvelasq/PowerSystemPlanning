using PowerSystemPlanning;
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

namespace PowerSystemPlanningWpfApp.LDC
{
    /// <summary>
    /// Interaction logic for OPFLDCResultsControl.xaml
    /// </summary>
    public partial class OPFLDCResultsControl : UserControl
    {
        PowerSystem _PowerSystem;
        LoadDurationCurveByBlocks _LoadDurationCurveByBlocks;
        LDCOPFModelSolver _LDCOPFModelSolver;
        LDCOPFModelResults _LDCOPFModelResults;
        LDCOPFModelResults LDCOPFModelResults
        {
            get { return _LDCOPFModelResults; }
            set
            {
                this._LDCOPFModelResults = this._LDCOPFModelSolver.LDCOPFResults;
                //Databinding
                this.tbTotalCost.DataContext = this.LDCOPFModelResults.ObjVal;
                this.lblCurrStateMsg.DataContext = this.LDCOPFModelResults.CurrentStateMessage;
                this.dgNodalResults.DataContext = this.LDCOPFModelResults.NodeLDCOPFResults;
            }
        }
        
        public PowerSystem PowerSystem
        {
            get
            {
                return _PowerSystem;
            }

            set
            {
                _PowerSystem = value;
                this._LDCOPFModelSolver = new LDCOPFModelSolver(this.PowerSystem, _LoadDurationCurveByBlocks);
            }
        }

        public OPFLDCResultsControl()
        {
            InitializeComponent();
            this._LoadDurationCurveByBlocks = new LoadDurationCurveByBlocks();
            //Default load blocks
            _LoadDurationCurveByBlocks.DurationBlocks.Add(new LoadBlock(6000, 0.4));
            _LoadDurationCurveByBlocks.DurationBlocks.Add(new LoadBlock(2000, 0.6));
            _LoadDurationCurveByBlocks.DurationBlocks.Add(new LoadBlock(760, 1));
            //Bind load blocks
            this.dgLDC.DataContext = _LoadDurationCurveByBlocks.DurationBlocks;
        }

        private void btnRunLdcOpf_Click(object sender, RoutedEventArgs e)
        {
            //builds the model
            this._LDCOPFModelSolver.Build();
            //solves the model
            this._LDCOPFModelSolver.Solve();
            //binds nodal results to the nodal datagrid
            this.LDCOPFModelResults = this._LDCOPFModelSolver.LDCOPFResults;
        }

        private void dgLDCMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Menu item of the LDC datagrid, clicked to view detailed results for a single block of the LDC
            int selected_index = this.dgLDC.SelectedIndex;
            OPF.OPFResultsWindow opfResultsWindow = new OPF.OPFResultsWindow();
            opfResultsWindow.OPFResults = this._LDCOPFModelResults.OpfResultsByBlock[selected_index];
            opfResultsWindow.Show();
        }
    }
}
