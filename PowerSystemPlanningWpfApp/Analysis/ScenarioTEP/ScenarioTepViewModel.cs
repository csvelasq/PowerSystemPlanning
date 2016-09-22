using PowerSystemPlanning.PlanningModels;
using PowerSystemPlanning.PlanningModels.Planning;
using PowerSystemPlanning.Solvers.LDCOPF;
using PowerSystemPlanningWpfApp.Analysis;
using PowerSystemPlanningWpfApp.Analysis.LDC;
using Prism.Commands;
using Prism.Mvvm;
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
    public class ScenarioTepViewModel : BindableBase
    {
        public ScenarioTEPModel _MyScenarioTEPModel;
        public ScenarioTEPModel MyScenarioTEPModel
        {
            get { return _MyScenarioTEPModel; }
            set
            {
                SetProperty<ScenarioTEPModel>(ref _MyScenarioTEPModel, value);
                MyTEPlanDetailViewModel = new TransmissionExpansionPlanDetailViewModel(MyScenarioTEPModel.MyCandidateTransmissionLines, true);
            }
        }

        TransmissionExpansionPlanDetailViewModel _MyTEPlanDetailViewModel;
        /// <summary>
        /// The view model for viewing and editing the transmission expansion plan currently under analysis.
        /// </summary>
        public TransmissionExpansionPlanDetailViewModel MyTEPlanDetailViewModel
        {
            get { return _MyTEPlanDetailViewModel; }
            set
            {
                SetProperty<TransmissionExpansionPlanDetailViewModel>(ref _MyTEPlanDetailViewModel, value);
            }
        }

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
                SetProperty<TransmissionExpansionPlan>(ref _MyTransmissionExpansionPlan, value);
            }
        }

        public TransmissionExpansionPlanLDCResultsForOneScenario _CurrentlySelectedScenarioLDC;
        public TransmissionExpansionPlanLDCResultsForOneScenario CurrentlySelectedScenarioLDC
        {
            get { return _CurrentlySelectedScenarioLDC; }
            set
            {
                SetProperty<TransmissionExpansionPlanLDCResultsForOneScenario>(ref _CurrentlySelectedScenarioLDC, value);
                if (_CurrentlySelectedScenarioLDC != null)
                {
                    MyOPFLDCViewModel.MyLDCOPFModelResults = _CurrentlySelectedScenarioLDC.DetailedLDCOPFModelResults;
                }
            }
        }

        public OPFLDCViewModel MyOPFLDCViewModel { get; private set; }

        public ICommand SolveScenarioLDCOPFCommand { get; private set; }

        public ICommand EnumerateTransmissionExpansionPlans { get; private set; }

        public ICommand EvaluateEnumeratedTransmissionExpansionPlans { get; private set; }

        public ICommand DgTepEnum_DoubleClick { get; private set; }

        /// <summary>
        /// The results of evaluating the currently selected transmission expansion plan, under several future scenarios
        /// </summary>
        public TransmissionExpansionPlanScenarioDetailedResults MyTEPDetailedResults
        {
            get { return _MyTEPDetailedResults; }
            set
            {
                SetProperty<TransmissionExpansionPlanScenarioDetailedResults>(ref _MyTEPDetailedResults, value);
            }
        }

        TransmissionExpansionPlanScenarioDetailedResults _MyTEPDetailedResults;

        List<TransmissionExpansionPlan> _AllTEPAlternatives;
        public List<TransmissionExpansionPlan> AllTEPAlternatives
        {
            get { return _AllTEPAlternatives; }
            set
            {
                SetProperty<List<TransmissionExpansionPlan>>(ref _AllTEPAlternatives, value);
            }
        }

        // TODO binding to detailed opf ldc results for the selected scenario
        //public LDCOPFModelResults r;

        public ScenarioTepViewModel()
        {
            MyOPFLDCViewModel = new OPFLDCViewModel();
            SolveScenarioLDCOPFCommand = new DelegateCommand(SolveScenarioLDCOPF);
            EnumerateTransmissionExpansionPlans = new DelegateCommand(RunEnumerateTransmissionExpansionPlans);
            EvaluateEnumeratedTransmissionExpansionPlans = new DelegateCommand(RunEvaluateEnumeratedTransmissionExpansionPlans);
            DgTepEnum_DoubleClick = new DelegateCommand(ShowDetailedTEPResultsForOneAlternative);
        }

        public ScenarioTepViewModel(ScenarioTEPModel s) : this()
        {
            MyScenarioTEPModel = s;
        }

        public void SolveScenarioLDCOPF()
        {
            // Build the plan
            MyTransmissionExpansionPlan = new TransmissionExpansionPlan(MyTEPlanDetailViewModel.MySelectedCandidateTransmissionLines.ToList<CandidateTransmissionLine>(), MyScenarioTEPModel, 
                new PowerSystemPlanning.Models.Planning.ScenarioTEP.MOOScenarioTEP(MyScenarioTEPModel));
            // Evaluate total costs under each scenario
            MyTEPDetailedResults = MyTransmissionExpansionPlan.EvaluateScenarios(true);
        }

        public void RunEnumerateTransmissionExpansionPlans()
        {
            AllTEPAlternatives = MyScenarioTEPModel.EnumerateAlternativeTransmissionExpansionPlans();
        }

        public void RunEvaluateEnumeratedTransmissionExpansionPlans()
        {
            AllTEPAlternatives = MyScenarioTEPModel.EnumerateAlternativeTransmissionExpansionPlans();
            //Evaluate the alternatives
            foreach (var tePlan in AllTEPAlternatives)
            {
                //Evaluate the current alternative, building detailed results
                tePlan.EvaluateScenarios(false);
            }
            //Fire event to enable view to generate as many columns as necessary
            OnAllTepAlternativesEvaluated?.Invoke();
        }

        public event Action OnAllTepAlternativesEvaluated;

        public void ShowDetailedTEPResultsForOneAlternative()
        {
            // TODO make this window show the selected plan
            ScenarioTEPLDCInspectWindow tepDetailsWindow = new ScenarioTEPLDCInspectWindow();
            tepDetailsWindow.DataContext = this;
            tepDetailsWindow.Show();
        }
    }
}
