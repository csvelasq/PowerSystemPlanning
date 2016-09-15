using PowerSystemPlanning;
using PowerSystemPlanning.PlanningModels;
using PowerSystemPlanning.PlanningModels.Planning;
using PowerSystemPlanning.Solvers.LDCOPF;
using PowerSystemPlanning.Solvers.OPF;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PowerSystemPlanningWpfApp.Model
{
    public class ScenarioTEPViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Example implementation from: https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ScenarioTEPModel _MyScenarioTEPModel;
        /// <summary>
        /// The TEP scenario model of the power system.
        /// </summary>
        public ScenarioTEPModel MyScenarioTEPModel
        {
            get
            {
                return _MyScenarioTEPModel;
            }
            set
            {
                if (_MyScenarioTEPModel != value)
                {
                    _MyScenarioTEPModel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        PowerSystemScenario _currentlySelectedScenario;
        /// <summary>
        /// Currently selected (in GUI) scenario (for editing and analysis purposes).
        /// </summary>
        public PowerSystemScenario currentlySelectedScenario
        {
            get { return _currentlySelectedScenario; }
            set
            {
                if (_currentlySelectedScenario != value)
                {
                    _currentlySelectedScenario = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Build and solve an LDC OPF on the currently selected scenario.
        /// </summary>
        public ICommand LDCOPFSolveCommand { get; private set; }

        LDCOPFModelSolver _MyLDCOPFModelSolver;
        LDCOPFModelResults _MyLDCOPFModelResults;
        /// <summary>
        /// LDC OPF results, to be bound to the corresponding results tab
        /// </summary>
        public LDCOPFModelResults MyLDCOPFModelResults
        {
            get { return _MyLDCOPFModelResults; }
            set
            {
                if (_MyLDCOPFModelResults != value)
                {
                    _MyLDCOPFModelResults = value;
                    NotifyPropertyChanged();
                }
            }
        }

        OPFModelResultForLDC _selectedLoadBlockInLDCOPFResults;
        /// <summary>
        /// Currently selected load block (in LDCOPF Tab) for inspecting detailed OPF results.
        /// </summary>
        public OPFModelResultForLDC selectedLoadBlockInLDCOPFResults
        {
            get { return _selectedLoadBlockInLDCOPFResults; }
            set
            {
                if (_selectedLoadBlockInLDCOPFResults != value)
                {
                    _selectedLoadBlockInLDCOPFResults = value;
                    NotifyPropertyChanged();
                    if (value != null)
                    {
                        // focuses the 'OPF' detailed results tab
                        tabControlPowerSystem_SelectedIndex = 1;
                    }
                }
            }
        }

        /// <summary>
        /// Currently selected tab in the frontend (allows changing the focused tab from within this view-model)
        /// </summary>
        public int tabControlPowerSystem_SelectedIndex { get; set; }

        /// <summary>
        /// Initializes a model with default (arbitrary) parameters
        /// </summary>
        public ScenarioTEPViewModel() : this(new ScenarioTEPModel()) { }

        public ScenarioTEPViewModel(ScenarioTEPModel myScenarioTEPModel)
        {
            MyScenarioTEPModel = myScenarioTEPModel;
            LDCOPFSolveCommand = new DelegateCommand(RunLdcOpf, CanRunLdcOpf);
        }

        /// <summary>
        /// Builds and solves an LDC OPF model based on the currently selected scenario
        /// </summary>
        public void RunLdcOpf()
        {
            //builds the model
            _MyLDCOPFModelSolver = new LDCOPFModelSolver(currentlySelectedScenario.MyPowerSystem, MyScenarioTEPModel.MyLoadDurationCurve);
            //solves the model
            _MyLDCOPFModelSolver.Solve();
            //binds results
            MyLDCOPFModelResults = _MyLDCOPFModelSolver.MyLDCOPFResults;
        }
        /// <summary>
        /// Determines whether an LDC OPF model can be built and solved (depending on whether there is a selected power system).
        /// </summary>
        /// <returns></returns>
        public bool CanRunLdcOpf()
        {
            // TODO should also verify internal consistency of the selected scenario
            return (currentlySelectedScenario != null);
        }

        /// <summary>
        /// Creates a new scenario tep view model with default (arbitrary) parameters for the power system data model.
        /// </summary>
        /// <returns></returns>
        public static ScenarioTEPViewModel CreateDefaultScenarioTEPModel()
        {
            ScenarioTEPViewModel MyScenarioTEPViewModel = new ScenarioTEPViewModel();
            ScenarioTEPModel MyScenarioTEPModel = MyScenarioTEPViewModel.MyScenarioTEPModel;
            //Default load duration curve
            LoadDurationCurveByBlocks defaultLoadDurationCurve = new LoadDurationCurveByBlocks();
            defaultLoadDurationCurve.DurationBlocks.Add(new LoadBlock(6000, 0.4));
            defaultLoadDurationCurve.DurationBlocks.Add(new LoadBlock(2000, 0.6));
            defaultLoadDurationCurve.DurationBlocks.Add(new LoadBlock(760, 1));
            //Default name and discount rate
            MyScenarioTEPModel.Name = "Unnamed power system model";
            MyScenarioTEPModel.YearlyDiscountRate = 0.07;
            MyScenarioTEPModel.MyLoadDurationCurve = defaultLoadDurationCurve;
            MyScenarioTEPModel.TargetPlanningYear = 10;
            //Adds an empty scenario
            MyScenarioTEPModel.MyScenarios.Add(new PowerSystemScenario("Unnamed scenario", new PowerSystem()));
            return MyScenarioTEPViewModel;
        }
    }
}
