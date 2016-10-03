using PowerSystemPlanning.Models.Planning.LDC;
using PowerSystemPlanning.Models.Planning.ScenarioTEP;
using PowerSystemPlanning.Solvers.LDCOPF;
using PowerSystemPlanning.Solvers.LDCOPF.LdcOpfResults;
using PowerSystemPlanning.Solvers.OPF.OpfResults;
using PowerSystemPlanningWpfApp.Analysis.OPF;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PowerSystemPlanningWpfApp.Analysis.LDC
{
    /// <summary>
    /// View-model for analizing results of the LDC OPF model (these results are already constructed, not built in this view-model).
    /// </summary>
    public class OPFLDCViewModel : BindableBase
    {
        LdcOpfModelResults _MyLDCOPFModelResults;
        public LdcOpfModelResults MyLDCOPFModelResults
        {
            get { return _MyLDCOPFModelResults; }
            set
            {
                SetProperty<LdcOpfModelResults>(ref this._MyLDCOPFModelResults, value);
            }
        }

        private OPFModelResult currentlySelectedLoadBlockResults;

        /// <summary>
        /// Handles a double click in the datagrid with summarized results for each LDC block (shows detailed OPF results window).
        /// </summary>
        public ICommand DgLDC_DoubleClick { get; private set; }

        public OPFModelResult CurrentlySelectedLoadBlockResults
        {
            get { return currentlySelectedLoadBlockResults; }
            set { SetProperty<OPFModelResult>(ref currentlySelectedLoadBlockResults, value); }
        }

        public OPFLDCViewModel()
        {
            DgLDC_DoubleClick = new DelegateCommand(ViewDetailedOpfResults, delegate () { return CurrentlySelectedLoadBlockResults != null; });
        }

        private void ViewDetailedOpfResults()
        {
            OPFResultsWindow opfDetailsWindow = new OPFResultsWindow();
            opfDetailsWindow.DataContext = CurrentlySelectedLoadBlockResults;
            opfDetailsWindow.Show();
        }
    }

    public class RunOPFLDCViewModel : OPFLDCViewModel
    {
        private LDCOPFModelSolver MyLDCOPFModelSolver;

        public LoadDurationCurveByBlocks MyLoadDurationCurve { get; set; }

        private BindingList<PowerSystemScenario> _MyScenarios;

        private PowerSystemScenario _CurrentlySelectedScenario;

        public BindingList<PowerSystemScenario> MyScenarios
        {
            get { return _MyScenarios; }
            set { SetProperty(ref _MyScenarios, value); }
        }

        public PowerSystemScenario CurrentlySelectedScenario
        {
            get { return _CurrentlySelectedScenario; }
            set { SetProperty(ref _CurrentlySelectedScenario, value); }
        }

        public ICommand LDCOPFSolveCommand { get; private set; }

        public RunOPFLDCViewModel() : base()
        {
            LDCOPFSolveCommand = new DelegateCommand(SolveLDCOPF, delegate () { return CurrentlySelectedScenario != null; });
        }

        public RunOPFLDCViewModel(BindingList<PowerSystemScenario> myScenarios, LoadDurationCurveByBlocks myLoadDurationCurve) : this()
        {
            MyScenarios = myScenarios;
            MyLoadDurationCurve = myLoadDurationCurve;
        }

        private void SolveLDCOPF()
        {
            MyLDCOPFModelSolver = new LDCOPFModelSolver(CurrentlySelectedScenario.MyPowerSystem, MyLoadDurationCurve);
            MyLDCOPFModelSolver.Solve();
            MyLDCOPFModelResults = MyLDCOPFModelSolver.MyLDCOPFResults;
        }
    }
}
