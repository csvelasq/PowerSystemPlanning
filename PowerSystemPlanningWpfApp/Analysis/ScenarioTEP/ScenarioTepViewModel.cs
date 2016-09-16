using PowerSystemPlanning.PlanningModels;
using PowerSystemPlanning.PlanningModels.Planning;
using PowerSystemPlanning.Solvers.LDCOPF;
using PowerSystemPlanningWpfApp.Analysis.TEP;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PowerSystemPlanningWpfApp.Analysis.ScenarioTEP
{
    public class ScenarioTepViewModel : INotifyPropertyChanged
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
                    _MyTEPlanDetailViewModel = new TransmissionExpansionPlanDetailViewModel(MyScenarioTEPModel.MyCandidateTransmissionLines, true);
                    NotifyPropertyChanged();
                }
            }
        }

        TransmissionExpansionPlanDetailViewModel _MyTEPlanDetailViewModel;
        /// <summary>
        /// The view model for viewing and editing the transmission expansion plan currently under analysis.
        /// </summary>
        public TransmissionExpansionPlanDetailViewModel MyTEPlanDetailViewModel { get { return _MyTEPlanDetailViewModel; } }

        TransmissionExpansionPlan _MyTransmissionExpansionPlan;
        /// <summary>
        /// The transmission expansion plan which is currently selected in the GUI, for the purpose of inspecting detailed results.
        /// </summary>
        public TransmissionExpansionPlan MyTransmissionExpansionPlan
        {
            get
            {
                return _MyTransmissionExpansionPlan;
            }
            set
            {
                if (_MyTransmissionExpansionPlan != value)
                {
                    _MyTransmissionExpansionPlan = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand SolveScenarioLDCOPFCommand { get; private set; }

        public ICommand EnumerateTransmissionExpansionPlans { get; private set; }

        public ICommand EvaluateEnumeratedTransmissionExpansionPlans { get; private set; }

        /// <summary>
        /// The results of evaluating the currently selected transmission expansion plan, under several future scenarios
        /// </summary>
        public TransmissionExpansionPlanScenarioDetailedResults MyTEPDetailedResults
        {
            get
            {
                return _MyTEPDetailedResults;
            }

            set
            {
                if (_MyTEPDetailedResults != value)
                {
                    _MyTEPDetailedResults = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        TransmissionExpansionPlanScenarioDetailedResults _MyTEPDetailedResults;

        List<TransmissionExpansionPlan> _AllTEPAlternatives;
        public List<TransmissionExpansionPlan> AllTEPAlternatives
        {
            get
            {
                return _AllTEPAlternatives;
            }

            set
            {
                if (_AllTEPAlternatives != value)
                {
                    _AllTEPAlternatives = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // TODO binding to detailed opf ldc results for the selected scenario
        //public LDCOPFModelResults r;

        public ScenarioTepViewModel()
        {
            SolveScenarioLDCOPFCommand = new DelegateCommand(SolveScenarioLDCOPF);
            TEPlansAreEnumerated = false;
            EnumerateTransmissionExpansionPlans = new DelegateCommand(RunEnumerateTransmissionExpansionPlans);
            //EvaluateEnumeratedTransmissionExpansionPlans = new DelegateCommand(RunEvaluateEnumeratedTransmissionExpansionPlans, CanExecuteEvaluateEnumeratedTransmissionExpansionPlans);
        }

        public ScenarioTepViewModel(ScenarioTEPModel s) : this()
        {
            MyScenarioTEPModel = s;
        }

        public void SolveScenarioLDCOPF()
        {
            // Build the plan
            MyTransmissionExpansionPlan = new TransmissionExpansionPlan(MyTEPlanDetailViewModel.MySelectedCandidateTransmissionLines.ToList<CandidateTransmissionLine>(), MyScenarioTEPModel);
            // Evaluate total costs under each scenario
            MyTransmissionExpansionPlan.EvaluateObjectives();
            // Build detailed results for binding to GUI
            MyTEPDetailedResults = MyTransmissionExpansionPlan.BuildDetailedTEPScenariosResults();
        }

        bool TEPlansAreEnumerated;
        public void RunEnumerateTransmissionExpansionPlans()
        {
            AllTEPAlternatives = MyScenarioTEPModel.EnumerateAlternativeTransmissionExpansionPlans();
            TEPlansAreEnumerated = true;
        }

        public void RunEvaluateEnumeratedTransmissionExpansionPlans()
        {
        }
        public bool CanExecuteEvaluateEnumeratedTransmissionExpansionPlans()
        {
            return TEPlansAreEnumerated;
        }
    }
}
